# Stage 1: Build React app
FROM node:20-alpine AS builder
WORKDIR /app

COPY package*.json ./
RUN npm ci

COPY . .
RUN npm run build

# Stage 2: NGINX to serve React + runtime env injection
FROM nginx:stable-alpine

# Copy built React app
COPY --from=builder /app/build /usr/share/nginx/html

# Copy your nginx config
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy env-config template and entrypoint script
COPY public/env-config.js.template /usr/share/nginx/html/env-config.js.template
COPY entrypoint.sh /entrypoint.sh

RUN chmod +x /entrypoint.sh

EXPOSE 80

ENTRYPOINT ["/entrypoint.sh"]
CMD ["nginx", "-g", "daemon off;"]