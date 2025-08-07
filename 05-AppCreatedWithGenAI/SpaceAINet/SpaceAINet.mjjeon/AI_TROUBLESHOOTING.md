# AI Troubleshooting Guide

## AI가 "stop"만 반복하는 문제 해결

### 1. Ollama 설치 및 실행 확인
```bash
# Ollama 설치 (Windows)
# https://ollama.com/download 에서 다운로드

# Ollama 서비스 시작
ollama serve

# 별도 터미널에서 모델 다운로드
ollama pull llama3.2
# 또는
ollama pull phi3
# 또는  
ollama pull codellama

# 모델 목록 확인
ollama list
```

### 2. 연결 테스트
```bash
# PowerShell에서 Ollama API 테스트
Invoke-RestMethod -Uri "http://localhost:11434/api/tags" -Method Get
```

### 3. 수동 AI 테스트
```bash
# 터미널에서 직접 테스트
ollama run llama3.2 "Choose one: move_left, move_right, fire, stop"
```

### 4. 일반적인 문제와 해결책

**문제: "Connection refused"**
- 해결: `ollama serve` 명령어로 서비스 시작

**문제: "Model not found"**  
- 해결: `ollama pull llama3.2` 명령어로 모델 다운로드

**문제: 응답이 느림**
- 해결: 더 작은 모델 사용 (`phi3`, `tinyllama`)

**문제: 이상한 응답**
- 해결: 프롬프트 개선 필요

### 5. 디버그 모드 실행
현재 코드에 디버그 로깅이 추가되어 있습니다:
- `[DEBUG]` 메시지를 확인하여 정확한 문제 파악
- Console에서 연결 상태, AI 응답, 파싱 결과 확인 가능

### 6. 대안 모델들
```bash
# 빠른 모델 (작은 크기)
ollama pull tinyllama

# 중간 성능
ollama pull phi3

# 좋은 성능 (큰 크기)  
ollama pull llama3.2
```
