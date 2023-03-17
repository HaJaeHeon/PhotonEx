# PhotonEx

총 소요시간 : 약 2주(?) // 최근에 5일정도 해서 매칭시스템으로 변경

Photon을 이용한 매칭 시스템을 구축하였습니다.

매칭 조건은 임의로 '사람수' 와 '게임 시간'으로 나누었습니다.

매칭이 되면 LoadLevel을 이용하여 Scene을 이동합니다.

GameScene에서 플레이어 머리 위에는 초기에 설정한 NickName이 쓰여있게 만들었고 가까이 가야 보이네요
이름표는 항상 플레이어의 앞을 바라보게끔 설정

채팅창은 photonChat을 이용해 PunRPC를 통해 같은 방 내의 모든 플레이어에게 전달

SettingUI에는 Sensitivity와 Resolution만 만들어놓았습니다.
