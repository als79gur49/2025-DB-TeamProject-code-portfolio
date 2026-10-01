using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EntityUI : MonoBehaviour
{
    [SerializeField]
    private Slider hpSlider;

    [SerializeField]
    private Entity entity;


    private void OnEnable()
    {
        entity.onTakeDamage.AddListener(UpdateHPHUD);

        hpSlider.value = 1;
    }

    public void UpdateHPHUD(float current, float max)
    {
        if(current <= 0)
        {
            hpSlider.value = 0;

            return;
        }

        hpSlider.value = current / max;
    }
    private void Update()
    {
        if (Camera.main != null)
        {
            Vector3 camPos = Camera.main.transform.position;

            Vector3 t = new Vector3(hpSlider.gameObject.transform.position.x,
                camPos.y, camPos.z);
            hpSlider.gameObject.transform.LookAt(t);
        }
    }
}
