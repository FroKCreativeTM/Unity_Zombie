using UnityEngine;
using UnityEngine.UI; // UI 관련 코드

// 플레이어 캐릭터의 생명체로서의 동작을 담당
public class PlayerHealth : LivingEntity {
    public Slider healthSlider; // 체력을 표시할 UI 슬라이더

    public AudioClip deathClip; // 사망 소리
    public AudioClip hitClip; // 피격 소리
    public AudioClip itemPickupClip; // 아이템 습득 소리

    private AudioSource playerAudioPlayer; // 플레이어 소리 재생기
    private Animator playerAnimator; // 플레이어의 애니메이터

    private PlayerMovement playerMovement; // 플레이어 움직임 컴포넌트
    private PlayerShooter playerShooter; // 플레이어 슈터 컴포넌트

    private void Awake() {
        // 사용할 컴포넌트를 가져오기
        playerAnimator = GetComponent<Animator>();
        playerAudioPlayer = GetComponent<AudioSource>();

        playerMovement = GetComponent<PlayerMovement>();
        playerShooter = GetComponent<PlayerShooter>();
    }

    // PlayerHealth 스크립트 활성화될 때마다
    // 체력 상태를 리셋처리하게
    protected override void OnEnable() {
        // LivingEntity의 OnEnable() 실행 (상태 초기화)
        base.OnEnable();

        // 체력 슬라이더를 활성화한다.
        healthSlider.gameObject.SetActive(true);
        // 그 다음 체력 슬라이더의 최댓값을 기본 체력값으로 변경한다.
        healthSlider.maxValue = startingHealth;
        // 그리고 체력 슬라이더의 값을 현재 체력값으로 변경한다.
        healthSlider.value = health;

        // 플레이어 조작을 받는 컴포넌트 활성화
        playerMovement.enabled = true;
        playerShooter.enabled = true;
    }

    // 체력 회복
    public override void RestoreHealth(float newHealth) {
        // LivingEntity의 RestoreHealth() 실행 (체력 증가)
        base.RestoreHealth(newHealth);

        // 갱신된 체력으로 체력 슬라이더 갱신
        healthSlider.value = health;
    }

    // 데미지 처리
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitDirection) {

        if(!dead)
        {
            // 사망하지 않은 경우에만 효과음 재생
            playerAudioPlayer.PlayOneShot(hitClip);
        }
        // LivingEntity의 OnDamage() 실행(데미지 적용)
        base.OnDamage(damage, hitPoint, hitDirection);
        // 갱신된 체력을 체력 슬라이더에 적용한다.
        healthSlider.value = health;
    }

    // 사망 처리
    public override void Die() {
        // LivingEntity의 Die() 실행(사망 적용)
        base.Die();

        // 체력 슬라이더 비활성화
        healthSlider.gameObject.SetActive(false);

        // 사망음 재생
        playerAudioPlayer.PlayOneShot(deathClip);
        // 애니메이터의 Die 트리거를 재생시킨다.
        playerAnimator.SetTrigger("Die");

        // 플레이어 조작을 받는 모든 컴포넌트를 비활성화 한다.
        playerMovement.enabled = false;
        playerShooter.enabled = false;
    }

    private void OnTriggerEnter(Collider other) {
        // 아이템과 충돌한 경우 해당 아이템을 사용하는 처리

        // 사망하지 않은 경우에만 충돌 처리가 된다.
        if (!dead)
        {
            // 상대 아이템의 컴포넌트 정보를 가져온다.
            IItem item = other.GetComponent<IItem>();

            // 이 때 아이템에 대한 정보가 있는 경우
            if (item != null)
            {
                // Use 메소드를 실행해서 아이템 사용
                item.Use(gameObject);
                playerAudioPlayer.PlayOneShot(itemPickupClip);      // 아이템 습득 소리 재생
            }
        }
    }
}