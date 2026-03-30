using UnityEngine;

public class SlidePlayer : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 10f;
    public float slideTime = 1f;

    float timer = 0f;
    bool glisse = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            glisse = true;
            timer = slideTime;
        }

        if (glisse)
        {
            Vector3 direction = transform.forward;
            controller.Move(direction * speed * Time.deltaTime);

            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                glisse = false;
            }
        }
    }
}