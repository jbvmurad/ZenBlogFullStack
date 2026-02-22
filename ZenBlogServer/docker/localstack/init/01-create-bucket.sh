#!/usr/bin/env bash
set -euo pipefail

# LocalStack runs init scripts after the services are ready.
awslocal s3 mb s3://zenblog-local || true
