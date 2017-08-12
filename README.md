# Instructions

## Follow these steps to run the sample locally:

```console
dotnet restore
dotnet run
```
## Follow these steps to run this sample in a Linux environment:

### Start rabbitmq
```console
docker run -d -p 5672:5672 -p 15672:15672 --hostname my-rabbit --name some-rabbit rabbitmq:3-management
```

### Build and run dotnetapp
```console
docker build -t dotnetapp .
docker run -d -e RABBITMQ_URI=rabbitmq://guest:guest@172.17.0.2:5672 --name some-dotnetapp dotnetapp
```
### Verify your receiving messages.
```console
docker logs some-dotnetapp --follow
```

### Stop and clean up container.
```console
docker stop some-dotnetapp
docker rm some-dotnetapp
```


