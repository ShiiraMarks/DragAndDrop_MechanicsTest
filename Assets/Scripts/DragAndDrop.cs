using UnityEngine;
using System.Collections;

public class DragAndDrop : MonoBehaviour
{
    private Vector3 offset;
    private Camera mainCamera;
    public static bool isDraggingObject = false; // Флаг для блокировки камеры при перетаскивании

    public LayerMask placementLayer; // Слой разрешённых зон
    public float fallSpeed = 5f; // Скорость падения
    private bool isFalling = false;
    private Vector3 targetPosition;

    // Размеры объекта
    public Vector3 normalScale = Vector3.one; // Стандартный размер
    public Vector3 enlargedScale = Vector3.one * 1.2f; // Увеличенный размер
    public float scaleSpeed = 5f; // Скорость изменения размера

    // Аудио
    public AudioSource audioSource; // Источник звука
    public AudioClip impactSound; // Звук при касании зоны
    public AudioClip pickUpSound; // Звук при захвате объекта
    public AudioClip dropSound; // Звук при отпускании объекта

    private void Start()
    {
        mainCamera = Camera.main; // Получаем камеру
    }

    private void OnMouseDown()
    {
        isDraggingObject = true; // Устанавливаем флаг перетаскивания
        isFalling = false; // Останавливаем падение
        offset = transform.position - GetMouseWorldPosition();

        // Увеличиваем объект
        StopAllCoroutines();
        StartCoroutine(ScaleObject(transform.localScale, enlargedScale));

        // Проигрываем звук захвата
        if (audioSource != null && pickUpSound != null)
        {
            audioSource.PlayOneShot(pickUpSound);
        }
    }

    private void OnMouseDrag()
    {
        if (isDraggingObject)
        {
            transform.position = GetMouseWorldPosition() + offset; // Перемещение объекта за курсором
        }
    }

    private void OnMouseUp()
    {
        isDraggingObject = false; // Сбрасываем флаг
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero, Mathf.Infinity, placementLayer);

        if (hit.collider != null)
        {
            // Если объект в зоне, фиксируем его на текущей позиции
            targetPosition = transform.position;
            isFalling = false;

            // Возвращаем объект к нормальному размеру
            StopAllCoroutines();
            StartCoroutine(ScaleObject(transform.localScale, normalScale));

            // Проигрываем звук отпускания
            if (audioSource != null && dropSound != null)
            {
                audioSource.PlayOneShot(dropSound);
            }
        }
        else
        {
            // Если объект вне зоны, задаём цель падения вниз
            targetPosition = new Vector3(transform.position.x, transform.position.y - 10f, transform.position.z);
            isFalling = true;
        }
    }

    private void Update()
    {
        // Логика плавного падения объекта до зоны
        if (isFalling)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero, Mathf.Infinity, placementLayer);

            if (hit.collider != null)
            {
                // Объект достигает зоны
                transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
                isFalling = false;

                // Проигрываем звук касания зоны
                if (audioSource != null && impactSound != null)
                {
                    audioSource.PlayOneShot(impactSound);
                }

                // Возвращаем объект к нормальному размеру
                StopAllCoroutines();
                StartCoroutine(ScaleObject(transform.localScale, normalScale));
            }
            else
            {
                // Плавное движение вниз
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, fallSpeed * Time.deltaTime);
            }
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f; // Устанавливаем расстояние до камеры
        return mainCamera.ScreenToWorldPoint(mousePos);
    }

    private IEnumerator ScaleObject(Vector3 from, Vector3 to)
    {
        float progress = 0f;

        while (progress < 1f)
        {
            progress += Time.deltaTime * scaleSpeed;
            transform.localScale = Vector3.Lerp(from, to, progress);
            yield return null;
        }

        transform.localScale = to; // Убедимся, что объект достигает конечного размера
    }
}
