using UnityEngine;

public class CameraScroll : MonoBehaviour
{
    public Camera mainCamera; // Ссылка на камеру
    public float scrollSpeed = 5f; // Скорость прокрутки
    public float minX = -10f; // Левый предел прокрутки
    public float maxX = 10f; // Правый предел прокрутки

    private Vector3 dragOrigin; // Начальная точка при нажатии

    public void Update()
    {
        // Проверка на нажатие (ЛКМ / по экрану) и то что не перетаскивается ли объект
        if (Input.GetMouseButtonDown(0) && !DragAndDrop.isDraggingObject)
        {
            dragOrigin = Input.mousePosition; // Запоминаем начальную точку
        }

        if (Input.GetMouseButton(0) && !DragAndDrop.isDraggingObject)
        {
            Vector3 difference = mainCamera.ScreenToWorldPoint(dragOrigin) - mainCamera.ScreenToWorldPoint(Input.mousePosition);
            dragOrigin = Input.mousePosition;

            // Двигаем камеру влево/вправо
            Vector3 newPosition = mainCamera.transform.position + new Vector3(difference.x, 0, 0);
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX); // Ограничиваем движение в пределах по X Y
            mainCamera.transform.position = newPosition;
        }
    }
}
