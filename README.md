# 🏃 Tower Runner

![Unity](https://img.shields.io/badge/Unity-6000.4.8f1-black?style=for-the-badge&logo=unity)
![C%23](https://img.shields.io/badge/C%23-.NET-purple?style=for-the-badge&logo=csharp)
![Platform](https://img.shields.io/badge/Platform-Android-green?style=for-the-badge&logo=android)
![Language](https://img.shields.io/badge/Language-Korean-red?style=for-the-badge)
![Firebase](https://img.shields.io/badge/Firebase-Analytics%20%26%20Crashlytics-orange?style=for-the-badge&logo=firebase)

<img width="1024" height="500" alt="TowerRunnerGraphic" src="https://github.com/user-attachments/assets/f54f2740-6185-4822-b116-0ff719c3b2d6" />

**Tower Runner**는 Unity 기반으로 개발된 모바일 러닝 액션 게임 프로젝트입니다.  
플레이어는 끊임없이 생성되는 장애물과 구조물을 돌파하며 더 높은 점수를 목표로 달립니다.  
간단한 조작 속에서도 속도감 있는 플레이와 성장 시스템, 반복 도전의 재미를 제공하는 데 초점을 맞추었습니다.

---

[![Tower Runner Gameplay](https://img.youtube.com/vi/uGHfm-dL0qE/0.jpg)](https://www.youtube.com/shorts/uGHfm-dL0qE?feature=share)

---

## 📌 프로젝트 소개

- **장르**: 모바일 러닝 액션 (Running Action)
- **개발 인원**: 1인 개발
- **개발 환경**: Unity 6000.4.8f1 / Visual Studio 2022
- **사용 언어**: C#
- **플랫폼**: Android / IOS(준비중)
- **핵심 기술**: Addressables, Object Pooling, Local Save, Firebase
- **개발 목표**: 실제 출시 및 라이브 서비스 운영 경험을 위한 모바일 게임 프로젝트

---

## 🛠 주요 구현 기능

### 1. 플레이어 및 게임 플레이 시스템
- **Player Controller**  
  드래그를 통한 횡 이동, 터치를 통한 공격 모션 기반의 직관적인 러닝 액션 구현.

- **Score System**  
  실시간 점수 계산 및 최고 점수 저장 기능 구현.

- **Difficulty Scaling**  
  플레이 시간에 따라 장애물 속도와 난이도가 점진적으로 증가하도록 설계.

- **Game Result UI**  
  게임 종료 시 최종 점수 및 세부 데이터를 표시하는 결과 화면 구현.

---

### 2. 최적화 및 구조 설계
- **Object Pooling**  
  반복 생성되는 장애물 및 이펙트를 풀링 처리하여 성능 최적화.

- **Event Driven Architecture**  
  Dictionary 기반 이벤트 시스템을 활용하여 낮은 결합도의 구조 설계.

- **Singleton Managers**  
  게임 상태 및 UI를 효율적으로 관리하는 매니저 구조 구성.

- **Interface Based UI Initialization**  
  인터페이스 기반 자동 UI 초기화 시스템 구현.

---

### 3. 데이터 및 성장 시스템
- **Local Save System**  
  JSON 기반 로컬 저장 시스템 구현.

- **Upgrade System**  
  인게임 재화를 사용한 강화 성장 시스템 구현.

- **Item System**  
  아이템 구매&인벤토리 및 랜덤 습득을 통한 추가 효과 시스템 구현.

- **Gold & Reward Logic**  
  플레이 내역에 따른 재화 지급 및 성장 루프 구성.

---

### 4. 서비스 및 운영 대응
- **Firebase Integration**  
  Firebase Analytics 및 Crashlytics 연동.

- **Google Play Deployment**  
  Google Play 내부 테스트 및 출시 대응 진행.

- **Settings System**  
  사운드, 밝기, 데이터 초기화 등 설정 기능 구현.

---

## 📂 프로젝트 구조 (Core Folders)

```text
Assets/
├── Scripts/
│   ├── Managers/        # 게임 전반 관리 매니저
│   ├── Player/          # 플레이어 관련 로직
│   ├── UI/              # UI 시스템 및 패널
│   ├── Obstacle/        # 장애물 및 패턴 로직
│   ├── Events/          # 이벤트 시스템
│   ├── SaveSystem/      # 저장 및 데이터 관리
│   └── Enforce/         # 강화 및 성장 시스템
├── Resources/           # 리소스 데이터
└── Scenes/              # 게임 씬 구성
```

---

## 🎯 개발 포인트

- 단순한 조작 안에서도 반복 플레이 동기를 유도하는 성장 구조 설계
- 모바일 환경에서 안정적인 프레임 유지 및 경량화 고려
- 유지보수성을 고려한 이벤트 기반 구조 설계
- 실제 Google Play 출시 프로세스 및 운영 경험 확보

---

## 🔗 Links

- GitHub Repository  
  https://github.com/Namminu/TowerRunner/tree/_develop

- Gameplay Video  
  https://www.youtube.com/shorts/uGHfm-dL0qE?feature=share
