#!/usr/bin/env bash
set -euo pipefail

# Load .env if present (docker compose does this too, but we want it for the script)
if [ -f .env ]; then
  set -a
  # shellcheck disable=SC1091
  source .env
  set +a
fi

API_PORT=${ZENBLOG_API_PORT:-18080}
RABBIT_UI_PORT=${ZENBLOG_RABBITMQ_UI_PORT:-15682}
LOCALSTACK_PORT=${ZENBLOG_LOCALSTACK_PORT:-14566}
MAILHOG_UI_PORT=${ZENBLOG_MAILHOG_UI_PORT:-18025}

wait_http() {
  local url="$1"; local name="$2"; local retries=${3:-60}
  for ((i=1;i<=retries;i++)); do
    if curl -fsS "$url" >/dev/null 2>&1; then
      echo "[ok] $name -> $url"
      return 0
    fi
    echo "[wait] $name ($url) ... ($i/$retries)"
    sleep 2
  done
  echo "[fail] $name not reachable: $url" >&2
  exit 1
}

echo "Running smoke checks..."
wait_http "http://localhost:${API_PORT}/health" "API health"
wait_http "http://localhost:${API_PORT}/" "API root"
wait_http "http://localhost:${API_PORT}/scalar" "API docs (Scalar)"
wait_http "http://localhost:${LOCALSTACK_PORT}/_localstack/health" "LocalStack health"
wait_http "http://localhost:${RABBIT_UI_PORT}" "RabbitMQ UI"
wait_http "http://localhost:${MAILHOG_UI_PORT}" "MailHog UI"

echo "All smoke checks passed ✅"
