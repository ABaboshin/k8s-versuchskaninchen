```bash
# rabbitmq
docker build -t rabbitmq:local -f rabbitmq/Dockerfile rabbitmq
kubectl apply -f rabbitmq-deployment.yml

# prometheus
docker build -t rabbitmq-monitoring:local -f rabbitmq-monitoring/Dockerfile rabbitmq-monitoring
docker build -t prometheus:local -f prometheus/Dockerfile prometheus
kubectl apply -f monitoring-deployment.yml

# app
docker build -t app:local -f app/ProducerConsumer/Dockerfile app/ProducerConsumer
kubectl apply -f app-deployment.yml

# apiserver
clone and build https://github.com/kubernetes/sample-apiserver
kubectl apply -f metric-deployment.yml

# scaler
kubectl apply -f scale-deployment.yml

curl -XPOST -H 'Content-Type: application/json' http://localhost:8080/api/v1/namespaces/custom-metrics/services/custom-metrics-apiserver:http/proxy/write-metrics/namespaces/app/services/kubernetes/test-metric --data-raw '"90"'
```
