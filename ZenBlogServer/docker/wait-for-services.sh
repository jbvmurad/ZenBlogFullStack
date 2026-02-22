#!/usr/bin/env bash
set -euo pipefail

wait_for_port() {
  local host="$1"
  local port="$2"
  local name="$3"
  local retries="${4:-60}"

  for ((i=1; i<=retries; i++)); do
    if nc -z "$host" "$port" >/dev/null 2>&1; then
      echo "[wait] $name is reachable at $host:$port"
      return 0
    fi
    echo "[wait] waiting for $name ($host:$port) ... ($i/$retries)"
    sleep 2
  done

  echo "[wait] ERROR: $name not reachable after $retries attempts."
  exit 1
}

wait_for_http() {
  local url="$1"
  local name="$2"
  local retries="${3:-60}"

  for ((i=1; i<=retries; i++)); do
    if curl -fsS "$url" >/dev/null 2>&1; then
      echo "[wait] $name is reachable at $url"
      return 0
    fi
    echo "[wait] waiting for $name ($url) ... ($i/$retries)"
    sleep 2
  done

  echo "[wait] ERROR: $name not reachable after $retries attempts."
  exit 1
}

echo "[wait] Waiting for infrastructure services..."

wait_for_port postgres 5432 "PostgreSQL"
wait_for_port redis 6379 "Redis"
wait_for_port rabbitmq 5672 "RabbitMQ"
# Email is configured to use Gmail SMTP; we don't block startup on an external SMTP host.
wait_for_http "http://localstack:4566/_localstack/health" "LocalStack"

echo "[wait] All dependencies are up. Starting ZenBlog.API..."
exec dotnet ZenBlog.API.dll
