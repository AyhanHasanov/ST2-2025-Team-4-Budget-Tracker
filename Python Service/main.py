"""
Main entry point for the LLM Budget Advisor service.
Run this file to start the FastAPI server.

Usage:
    python main.py

Requirements:
    - Ollama must be running on http://localhost:11434
    - Model qwen3:1.7b must be installed in Ollama
    
To install the model:
    ollama pull qwen3:1.7b
"""

import uvicorn
import logging

# Configure logging
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s'
)
logger = logging.getLogger(__name__)

if __name__ == "__main__":
    logger.info("=" * 60)
    logger.info("Starting LLM Budget Advisor Service")
    logger.info("=" * 60)
    logger.info("Service will be available at: http://localhost:8000")
    logger.info("API Documentation: http://localhost:8000/docs")
    logger.info("Health Check: http://localhost:8000/health")
    logger.info("")
    logger.info("IMPORTANT:")
    logger.info("  - Ollama must be running on http://localhost:11434")
    logger.info("  - Model 'qwen3:1.7b' must be installed")
    logger.info("  - To install: ollama pull qwen3:1.7b")
    logger.info("=" * 60)
    
    # Run the FastAPI application
    uvicorn.run(
        "llm_service.main:app",
        host="0.0.0.0",
        port=8000,
        reload=True,
        log_level="info"
    )
