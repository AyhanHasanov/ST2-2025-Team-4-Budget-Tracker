# LLM Budget Advisor Service

This is a Python microservice that provides AI-powered expense summarization and budgeting advice using a local LLM (Large Language Model) via Ollama.

## Prerequisites

### 1. Install Ollama

Download and install Ollama from [https://ollama.ai](https://ollama.ai)

### 2. Pull the Required Model

After installing Ollama, open a terminal and run:

```bash
ollama pull qwen3:1.7b
```

This will download the Qwen 1.7B model, which is optimized for financial advice and analysis.

### 3. Verify Ollama is Running

Ollama should start automatically. You can verify it's running by checking:

```bash
curl http://localhost:11434/api/version
```

## Installation

1. Navigate to the Python Service directory:

```bash
cd "Python Service"
```

2. Create a virtual environment (recommended):

```bash
python -m venv venv
```

3. Activate the virtual environment:

**Windows:**
```bash
venv\Scripts\activate
```

**macOS/Linux:**
```bash
source venv/bin/activate
```

4. Install dependencies:

```bash
pip install -r llm_service/requirements.txt
```

## Running the Service

Start the service by running:

```bash
python main.py
```

The service will start on `http://localhost:8000`

## API Documentation

Once the service is running, you can access:

- **Interactive API Docs (Swagger)**: http://localhost:8000/docs
- **Alternative API Docs (ReDoc)**: http://localhost:8000/redoc
- **Health Check**: http://localhost:8000/health

## API Endpoints

### 1. POST /api/summarize

Generates a natural language summary of expenses.

**Request Body:**
```json
{
  "expenses": [
    {"category": "Food", "amount": 300.50},
    {"category": "Transport", "amount": 150.00}
  ],
  "budget": 1000.00
}
```

**Response:**
```json
{
  "summary": "Your spending is concentrated in Food ($300.50) and Transport ($150)...",
  "total_amount": 450.50,
  "expense_count": 2
}
```

### 2. POST /api/advice

Get personalized budgeting advice based on your expenses.

**Request Body:**
```json
{
  "question": "How can I reduce my monthly expenses?",
  "expenses": [
    {"category": "Food", "amount": 300.50},
    {"category": "Transport", "amount": 150.00}
  ],
  "budget": 1000.00
}
```

**Response:**
```json
{
  "advice": "To reduce expenses, consider meal prepping to lower food costs...",
  "question": "How can I reduce my monthly expenses?"
}
```

## Troubleshooting

### Error: 422 Unprocessable Content

This error occurs when the request body doesn't match the expected schema. Common causes:

1. **Missing required fields**: Ensure `expenses` and `budget` are provided
2. **Invalid amount values**: All amounts must be greater than 0
3. **Empty expenses array**: At least one expense item is required
4. **Invalid question**: For advice endpoint, question must be 5-500 characters

**Example of a valid request:**
```json
{
  "expenses": [
    {"category": "Food", "amount": 100.50}
  ],
  "budget": 500.00
}
```

### Error: 503 Service Unavailable

This means the service cannot connect to Ollama. Verify:

1. Ollama is running: Check http://localhost:11434
2. The model is installed: Run `ollama list` to see installed models
3. No firewall is blocking port 11434

### Error: 504 Gateway Timeout

The LLM is taking too long to respond. This is normal for the first request as the model loads into memory. Subsequent requests should be faster.

## Integration with ASP.NET Core MVC

The Python service is designed to work with the Budget Tracker MVC application. The MVC app communicates with this service via HTTP requests to provide AI-powered features.

### Configuration in MVC

In `appsettings.json`, ensure the LLM service URL is configured:

```json
{
  "LlmService": {
    "BaseUrl": "http://localhost:8000"
  }
}
```

## Development

To run in development mode with auto-reload:

```bash
python main.py
```

The service will automatically reload when you make changes to the code.

## Model Information

**Model**: Qwen 3 (1.7B parameters)
- Optimized for conversational AI and text generation
- Fast inference on consumer hardware
- Good balance between performance and resource usage
- Suitable for financial advice and analysis tasks

## Notes

- The service uses port 8000 by default
- Ollama uses port 11434 by default
- First request may take longer as the model loads
- Responses are limited to 500 tokens for faster generation
- Temperature is set to 0.3 for summaries (more focused) and 0.6 for advice (more creative)

## Support

If you encounter issues:

1. Check Ollama is running: http://localhost:11434
2. Verify model is installed: `ollama list`
3. Check service logs for detailed error messages
4. Ensure all dependencies are installed: `pip install -r llm_service/requirements.txt`

