```
docker build -t rabbitmq:local -f rabbitmq/Dockerfile rabbitmq
kubectl apply -f rabbitmq-deployment.yml
docker build -t rabbitmq-monitoring:local -f rabbitmq-monitoring/Dockerfile rabbitmq-monitoring
docker build -t prometheus:local -f prometheus/Dockerfile prometheus
kubectl apply -f monitoring-deployment.yml
```
