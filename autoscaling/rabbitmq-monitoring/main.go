package main

import (
	"encoding/json"
	"fmt"
	"log"
	"net/http"
	"os"
	"strings"
	"time"

	"github.com/prometheus/client_golang/prometheus"
	"github.com/prometheus/client_golang/prometheus/promauto"
	"github.com/prometheus/client_golang/prometheus/promhttp"
)

type Queue struct {
	Name      string `json:name`
	VHost     string `json:vhost`
	Messages  int    `json:messages`
	Consumers int    `json:consumers`
}

func GetBaseUrl() string {
	return os.Getenv("RABBITMQ_URL")
}

func GetUsername() string {
	return os.Getenv("RABBITMQ_USERNAME")
}

func GetPassword() string {
	return os.Getenv("RABBITMQ_PASSWORD")
}

func GetVHost() string {
	return "/"
}

func GetQueues() []Queue {
	client := &http.Client{}
	req, _ := http.NewRequest("GET", fmt.Sprintf("%s/api/queues/", GetBaseUrl()), nil)
	req.SetBasicAuth(GetUsername(), GetPassword())
	resp, _ := client.Do(req)

	value := make([]Queue, 0)
	json.NewDecoder(resp.Body).Decode(&value)

	return value
}

func EscapeTagValue(str string) string {
	res := strings.Trim(strings.ReplaceAll(strings.ReplaceAll(strings.ReplaceAll(str, ",", " "), "\n", " "), "\r", " "), "\r\n")
	res = res[0:min(len(res), 700)]
	return res
}

func min(a, b int) int {
	if a < b {
		return a
	}
	return b
}

func recordMetrics() {

	go func() {
		for {

			log.Print("Check Queues")
			queues := GetQueues()

			for _, queue := range queues {
				log.Printf("Found queue %s %s", queue.Name, queue.Messages)

				queueMessages.With(prometheus.Labels{"queue": EscapeTagValue(queue.Name)}).Set((float64)(queue.Messages))
			}

			time.Sleep(20 * time.Second)
		}
	}()
}

var (
	queueMessages = promauto.NewGaugeVec(prometheus.GaugeOpts{
		Name: "rabbbitmq_queue_messages_count",
		Help: "The current number of messages in a queue",
	}, []string{"queue"})
)

func main() {
	recordMetrics()

	http.Handle("/metrics/", promhttp.Handler())
	http.ListenAndServe(":2112", nil)
}
