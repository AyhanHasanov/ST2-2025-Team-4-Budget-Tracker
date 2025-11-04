"""
Test script for the LLM Budget Advisor service.
Run this script to verify the service is working correctly.

Usage:
    python test_service.py
"""

import requests
import json
import sys

BASE_URL = "http://localhost:8000"

def print_header(text):
    """Print a formatted header."""
    print("\n" + "=" * 60)
    print(f"  {text}")
    print("=" * 60)

def test_health_check():
    """Test the health check endpoint."""
    print_header("Testing Health Check")
    try:
        response = requests.get(f"{BASE_URL}/health", timeout=5)
        if response.status_code == 200:
            print("✓ Health check passed")
            print(f"  Response: {response.json()}")
            return True
        else:
            print(f"✗ Health check failed with status code: {response.status_code}")
            return False
    except requests.exceptions.ConnectionError:
        print("✗ Cannot connect to service. Is it running on http://localhost:8000?")
        return False
    except Exception as e:
        print(f"✗ Error: {e}")
        return False

def test_summarize():
    """Test the summarize endpoint."""
    print_header("Testing Summarize Endpoint")
    
    # Sample data
    request_data = {
        "expenses": [
            {"category": "Food", "amount": 350.75},
            {"category": "Transport", "amount": 120.00},
            {"category": "Entertainment", "amount": 80.50},
            {"category": "Utilities", "amount": 200.00}
        ],
        "budget": 1000.00
    }
    
    print("Request:")
    print(json.dumps(request_data, indent=2))
    
    try:
        response = requests.post(
            f"{BASE_URL}/api/summarize",
            json=request_data,
            timeout=120
        )
        
        if response.status_code == 200:
            print("\n✓ Summarize request successful")
            result = response.json()
            print(f"\nSummary:")
            print(f"  {result['summary']}")
            print(f"\nTotal Amount: ${result['totalAmount']:.2f}")
            print(f"Expense Count: {result['expenseCount']}")
            return True
        else:
            print(f"\n✗ Summarize failed with status code: {response.status_code}")
            print(f"  Error: {response.text}")
            return False
    except requests.exceptions.Timeout:
        print("\n✗ Request timed out. This might happen on first request as model loads.")
        print("  Try running the test again.")
        return False
    except Exception as e:
        print(f"\n✗ Error: {e}")
        return False

def test_advice():
    """Test the advice endpoint."""
    print_header("Testing Advice Endpoint")
    
    # Sample data
    request_data = {
        "question": "How can I reduce my monthly expenses?",
        "expenses": [
            {"category": "Food", "amount": 350.75},
            {"category": "Transport", "amount": 120.00},
            {"category": "Entertainment", "amount": 80.50}
        ],
        "budget": 500.00
    }
    
    print("Request:")
    print(json.dumps(request_data, indent=2))
    
    try:
        response = requests.post(
            f"{BASE_URL}/api/advice",
            json=request_data,
            timeout=120
        )
        
        if response.status_code == 200:
            print("\n✓ Advice request successful")
            result = response.json()
            print(f"\nQuestion: {result['question']}")
            print(f"\nAdvice:")
            print(f"  {result['advice']}")
            return True
        else:
            print(f"\n✗ Advice failed with status code: {response.status_code}")
            print(f"  Error: {response.text}")
            return False
    except requests.exceptions.Timeout:
        print("\n✗ Request timed out. This might happen on first request as model loads.")
        print("  Try running the test again.")
        return False
    except Exception as e:
        print(f"\n✗ Error: {e}")
        return False

def test_validation_errors():
    """Test that validation errors are handled correctly."""
    print_header("Testing Validation (422 Error Handling)")
    
    # Test with invalid data (empty expenses)
    invalid_data = {
        "expenses": [],
        "budget": 1000.00
    }
    
    print("Testing with empty expenses array (should return 422)...")
    try:
        response = requests.post(
            f"{BASE_URL}/api/summarize",
            json=invalid_data,
            timeout=10
        )
        
        if response.status_code == 422:
            print("✓ Validation error correctly returned (422)")
            return True
        else:
            print(f"✗ Expected 422 but got {response.status_code}")
            return False
    except Exception as e:
        print(f"✗ Error: {e}")
        return False

def main():
    """Run all tests."""
    print_header("LLM Budget Advisor Service Test Suite")
    print("This will test all endpoints of the service.")
    print("Make sure the service is running on http://localhost:8000")
    print("and Ollama is running with the qwen3:1.7b model.")
    
    results = []
    
    # Run tests
    results.append(("Health Check", test_health_check()))
    
    if not results[0][1]:
        print("\n" + "=" * 60)
        print("Service is not running. Please start it with: python main.py")
        print("=" * 60)
        sys.exit(1)
    
    results.append(("Validation Error Test", test_validation_errors()))
    results.append(("Summarize Endpoint", test_summarize()))
    results.append(("Advice Endpoint", test_advice()))
    
    # Print summary
    print_header("Test Results Summary")
    passed = sum(1 for _, result in results if result)
    total = len(results)
    
    for test_name, result in results:
        status = "✓ PASSED" if result else "✗ FAILED"
        print(f"{status}: {test_name}")
    
    print(f"\n{passed}/{total} tests passed")
    
    if passed == total:
        print("\n🎉 All tests passed! The service is working correctly.")
        return 0
    else:
        print("\n⚠️  Some tests failed. Check the output above for details.")
        return 1

if __name__ == "__main__":
    sys.exit(main())

