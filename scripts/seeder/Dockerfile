# Dockerfile
FROM node:18-alpine

# Set working directory
WORKDIR /app

# Copy package files
COPY package*.json ./

# Install dependencies
RUN npm install

# Copy application code
COPY . .

# Expose port (optional, for potential API server)
EXPOSE 3000

# Run the application
CMD ["node", "index.js"]
