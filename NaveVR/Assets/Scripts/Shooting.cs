using UnityEngine;

public class Shooting : MonoBehaviour
{
    [Header("OVRHand")]
    [Tooltip("Objeto que contenga OVRHand")] public OVRHand hand;
    [Header("Bullet")] public GameObject bulletPrefab;
    [Header("Fire Point")]
    [Tooltip("Punto de salida del disparo")] public Transform firePoint;
    public float bulletSpeed = 20f;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;
    private int spreadCount = 5;
    private float spreadAngle = 30f;

    // Update is called once per frame
    void Update()
    {
        if (hand != null && hand.IsTracked)
        {
            bool isPinchingI = hand.GetFingerIsPinching(OVRHand.HandFinger.Index);
            bool isPinchingM = hand.GetFingerIsPinching(OVRHand.HandFinger.Middle);

            if (isPinchingI && Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
            else if (isPinchingM && Time.time >= nextFireTime)
            {
                    Spread();
                    nextFireTime = Time.time + fireRate;
            }
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = firePoint.forward * bulletSpeed;
        }
        Destroy(bullet, 5f);
    }

    void Spread()
    {
        float angleStep = spreadAngle / (spreadCount - 1);
        float startingAngle = -spreadAngle / 2f;
        for(int i = 0; i < spreadCount; i++)
        {
            float currentAngle = startingAngle + (angleStep * i);
            Quaternion rotation = firePoint.rotation * Quaternion.Euler(0, currentAngle, 0);
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = bullet.transform.forward * bulletSpeed;
            }
            Destroy(bullet, 5f);
        }
    }
}
