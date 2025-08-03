#!/bin/sh
# Substitute environment variables into env-config.js before nginx starts
envsubst < /usr/share/nginx/html/env-config.js.template > /usr/share/nginx/html/env-config.js

# Start nginx in foreground
exec nginx -g 'daemon off;'