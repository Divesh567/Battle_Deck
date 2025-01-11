using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardFanLayout : MonoBehaviour
{
    public List<RectTransform> cardTransforms;
    public float spacing = 20f; // spacing between cards
    public float angle = 15f; // angle between cards
    public float radius = 300f; // radius of the fan

    void Start()
    {
       
    }

    public void ArrangeCards()
    {
        if (cardTransforms == null || cardTransforms.Count == 0) return;

        float startAngle = -(angle * (cardTransforms.Count - 1) / 2);

        for (int i = 0; i < cardTransforms.Count; i++)
        {
            float currentAngle = startAngle + angle * i;
            Vector3 position = new Vector3(Mathf.Cos(Mathf.Deg2Rad * currentAngle), Mathf.Sin(Mathf.Deg2Rad * currentAngle), 0) * radius;

            cardTransforms[i].anchoredPosition = position;
            cardTransforms[i].rotation = Quaternion.Euler(0, 0, currentAngle);
        }
    }

    void OnDrawGizmos()
    {
        // Visualize the layout in the editor
        if (cardTransforms == null || cardTransforms.Count == 0) return;

        float startAngle = -(angle * (cardTransforms.Count - 1) / 2);

        for (int i = 0; i < cardTransforms.Count; i++)
        {
            float currentAngle = startAngle + angle * i;
            Vector3 position = new Vector3(Mathf.Cos(Mathf.Deg2Rad * currentAngle), Mathf.Sin(Mathf.Deg2Rad * currentAngle), 0) * radius;

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + position, 10f);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            ArrangeCards();
        }
    }
}
