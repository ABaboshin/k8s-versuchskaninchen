FROM golang:alpine AS builder

RUN apk update && apk add --no-cache git
WORKDIR $GOPATH/src/mypackage/myapp/
COPY . .

RUN go get -d -v

RUN GOOS=linux GOARCH=amd64 CGO_ENABLED=0 go build -ldflags="-w -s" -o /go/bin/rabbitmq-monitoring

FROM scratch

COPY --from=builder /go/bin/rabbitmq-monitoring /go/bin/rabbitmq-monitoring
ENTRYPOINT ["/go/bin/rabbitmq-monitoring"]
