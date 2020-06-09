# Using prometheus kubernetes operator to autoscale

It is an example of using kubernetes prometheus operator to autoscale the consumer
based on amount of messages in a rabbitmq queue.

 - Build and start rabbitmq

```bash
docker build -t rabbitmq:local -f rabbitmq/Dockerfile rabbitmq
kubectl apply -f rabbitmq-deployment.yml
```
 - Build prometheus, sample exporter for rabbitmq, start them

```

docker build -t rabbitmq-monitoring:local -f rabbitmq-monitoring/Dockerfile rabbitmq-monitoring
docker build -t prometheus:local -f prometheus/Dockerfile prometheus
kubectl apply -f monitoring-deployment.yml
```

 - Build producer-consumer and deploy it

```bash
docker build -t app:local -f app/ProducerConsumer/Dockerfile app/ProducerConsumer
kubectl apply -f app-deployment.yml
```
 - Deploy prometheus operator

```bash
kubectl apply -f metric-deployment.yml
```

 - Deploy an autoscaler configuration for consumer (scale +1 for every 30 messages in the queue)

```bash
kubectl apply -f scale-deployment.yml
```
 - Produce 10000 messages

```bash
curl http://localhost:5000/api/publish/10000
```

 - Watch the scaling and cooling down after 5m

```bash
watch kubectl get hpa -n app
```
