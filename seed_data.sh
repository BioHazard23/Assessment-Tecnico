#!/bin/bash

API_URL="http://localhost:5050/api"

# Get admin token
TOKEN=$(curl -s -X POST "$API_URL/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@test.com","password":"Admin123!"}' | \
  grep -o '"token":"[^"]*"' | cut -d'"' -f4)

echo "Token obtained: ${TOKEN:0:30}..."

# Create courses and get their IDs
echo "Creating courses..."

C1=$(curl -s -X POST "$API_URL/courses" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"title":"Introduction to Python Programming"}' | grep -o '"id":"[^"]*"' | cut -d'"' -f4)
echo "Course 1: Python - $C1"

C2=$(curl -s -X POST "$API_URL/courses" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"title":"Advanced JavaScript & React"}' | grep -o '"id":"[^"]*"' | cut -d'"' -f4)
echo "Course 2: JavaScript - $C2"

C3=$(curl -s -X POST "$API_URL/courses" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"title":"PostgreSQL Database Mastery"}' | grep -o '"id":"[^"]*"' | cut -d'"' -f4)
echo "Course 3: PostgreSQL - $C3"

C4=$(curl -s -X POST "$API_URL/courses" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"title":"Clean Architecture with .NET 8"}' | grep -o '"id":"[^"]*"' | cut -d'"' -f4)
echo "Course 4: .NET - $C4"

C5=$(curl -s -X POST "$API_URL/courses" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"title":"Docker & Kubernetes Essentials"}' | grep -o '"id":"[^"]*"' | cut -d'"' -f4)
echo "Course 5: Docker - $C5"

# Add lessons to Course 1 (Python)
echo "Adding lessons to Python course..."
curl -s -X POST "$API_URL/courses/$C1/lessons" -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d '{"title":"Getting Started with Python"}' > /dev/null
curl -s -X POST "$API_URL/courses/$C1/lessons" -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d '{"title":"Variables and Data Types"}' > /dev/null
curl -s -X POST "$API_URL/courses/$C1/lessons" -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d '{"title":"Control Flow and Loops"}' > /dev/null
curl -s -X POST "$API_URL/courses/$C1/lessons" -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d '{"title":"Functions and Modules"}' > /dev/null

# Add lessons to Course 2 (JavaScript)
echo "Adding lessons to JavaScript course..."
curl -s -X POST "$API_URL/courses/$C2/lessons" -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d '{"title":"ES6+ Features Overview"}' > /dev/null
curl -s -X POST "$API_URL/courses/$C2/lessons" -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d '{"title":"React Components and Props"}' > /dev/null
curl -s -X POST "$API_URL/courses/$C2/lessons" -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d '{"title":"State Management with Hooks"}' > /dev/null

# Add lessons to Course 3 (PostgreSQL)
echo "Adding lessons to PostgreSQL course..."
curl -s -X POST "$API_URL/courses/$C3/lessons" -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d '{"title":"Database Fundamentals"}' > /dev/null
curl -s -X POST "$API_URL/courses/$C3/lessons" -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d '{"title":"SQL Queries and Joins"}' > /dev/null

# Add lessons to Course 4 (.NET)
echo "Adding lessons to .NET course..."
curl -s -X POST "$API_URL/courses/$C4/lessons" -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d '{"title":"Domain Layer Design"}' > /dev/null
curl -s -X POST "$API_URL/courses/$C4/lessons" -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d '{"title":"Application Layer Use Cases"}' > /dev/null
curl -s -X POST "$API_URL/courses/$C4/lessons" -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d '{"title":"Infrastructure with EF Core"}' > /dev/null

# Add lessons to Course 5 (Docker)
echo "Adding lessons to Docker course..."
curl -s -X POST "$API_URL/courses/$C5/lessons" -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d '{"title":"Docker Containers 101"}' > /dev/null
curl -s -X POST "$API_URL/courses/$C5/lessons" -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d '{"title":"Building Docker Images"}' > /dev/null

# Publish some courses
echo "Publishing courses with lessons..."
curl -s -X PATCH "$API_URL/courses/$C1/publish" -H "Authorization: Bearer $TOKEN" > /dev/null
curl -s -X PATCH "$API_URL/courses/$C2/publish" -H "Authorization: Bearer $TOKEN" > /dev/null
curl -s -X PATCH "$API_URL/courses/$C4/publish" -H "Authorization: Bearer $TOKEN" > /dev/null

echo ""
echo "✅ Seed data completed!"
echo "Created 5 courses with lessons"
echo "Published 3 courses"
