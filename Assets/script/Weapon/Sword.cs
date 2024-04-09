using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{

    public int damage;
    public GameObject effect;

    // Start is called before the first frame update
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle") || other.gameObject.layer == LayerMask.NameToLayer("Default"))
        {

            GameObject effectInstance = Instantiate(effect, transform.position, Quaternion.identity);

            // 일정 시간(예: 5초)이 지난 후에 effectInstance를 파괴합니다.
            Destroy(effectInstance, 0.3f);

            //Destroy(gameObject, 1.0f);
        }
    }
}
