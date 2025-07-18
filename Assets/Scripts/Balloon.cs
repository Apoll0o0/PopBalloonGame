using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Balloon : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector] public BalloonType type;           
    [HideInInspector] public BalloonGenerator generator; 

    Image img;
    AudioSource audioSrc;

    bool popped = false;

    void Awake()
    {
        img = GetComponent<Image>();
        audioSrc = GetComponent<AudioSource>();
    }

    public void Init(BalloonType t, BalloonGenerator gen, float velocity)
    {
        type = t;
        generator = gen;

        img.color = t.color;
        GetComponent<Rigidbody2D>().velocity = new Vector2(0f, velocity);
    }

    public void OnPointerClick(PointerEventData eventData) { Pop(); }

    void Pop()
    {
        if (popped || generator.IsMenu) return;
        popped = true;

        generator.OnBalloonPopped(type);

        AudioClip clip = type.popSfx ? type.popSfx : generator.defaultPopSfx;
        if (clip) audioSrc.PlayOneShot(clip);

        if (type.popVfx)
        {
            Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(null, transform.position);

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

            worldPos.z = 0f;

            GameObject vfx = Instantiate(type.popVfx, worldPos, Quaternion.identity);

            ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
            if (ps != null) ps.Play();

            Destroy(vfx, 2f);
            Debug.Log("POP efekt sahneye eklendi!");
        }

        img.enabled = false;
        Destroy(gameObject, clip ? clip.length : 0.2f);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("TopBoundary") || popped) return;

        generator.OnBalloonReachedTop(type);
        Destroy(gameObject);
    }
}
