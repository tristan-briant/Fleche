using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class dessineLigne : MonoBehaviour {

    Transform liaison, atome;
    Transform liaisons, atomes;

    void Start () {

        liaisons = transform.Find("Liaisons");
        atomes = transform.Find("Atomes");

    }
	
	void Update () {
    }


    public void Line(){
        Color c = new Color(0, 0, 0);


        Transform line = transform.Find("line"); // élimine les lignes qui existent
        while (line != null)
        {
            DestroyImmediate(line.gameObject);
            line = transform.Find("line"); // élimine les lignes qui existent
        }

        liaison = null;
        foreach (Transform child in liaisons) {
            if (child.GetComponent<Toggle>().isOn) { liaison = child; }
        }

        atome = null;
        foreach (Transform child in atomes)
        {
            if (child.GetComponent<Toggle>().isOn) { atome = child; }
        }

        if (liaison && atome) {

            DrawCurvedArrowBetween(liaison, atome);
           
        }
        

    }

    void DrawLine(Vector3 start, Vector3 end, Color color) // Juste une ligne simple (mais ne sert à rien)
    {
        GameObject myLine = new GameObject()
        {
            name = "line"
        };
 
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
		lr.startColor = lr.endColor = color;
		lr.startWidth = lr.endWidth = 0.1f;
        lr.SetPosition(0, start+new Vector3(0,0,-1));
        lr.SetPosition(1, end + new Vector3(0, 0, -1));
    }



    void DrawCurvedArrowBetween(Transform liaison, Transform atome) // Une flèche courbe entre une liaison et un atome
    {
        int NSample = 20;
        float h = 1.0f;
        float width = 0.1f;
        Color color = new Color(0, 0, 0);

        Vector3 start = liaison.position;
        Vector3 end = atome.position;

        Collider2D colliderAtome = atome.GetComponent<Collider2D>();
        Collider2D colliderLiaison = liaison.GetComponent<Collider2D>();

        
        GameObject Arrow = new GameObject()
        {
            name = "line"
        };
        Arrow.transform.parent = transform;

        GameObject Line = new GameObject();
        GameObject Head = new GameObject();

        Line.transform.parent = Arrow.transform;
        Head.transform.parent = Arrow.transform;

        Line.AddComponent<LineRenderer>();
        LineRenderer lr = Line.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.startColor = lr.endColor = color;
        lr.startWidth = lr.endWidth = width;
        lr.numCapVertices = 10;
        lr.positionCount = NSample; 

        Head.AddComponent<LineRenderer>();
        LineRenderer head = Head.GetComponent<LineRenderer>();
        head.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        head.startColor = head.endColor = color;
        head.startWidth = 3 * width; head.endWidth = 0;
        head.positionCount = 2;

        // Les maths pour calculer le centre, le rayon de courbure, et l'angle d'ouverture de la flèche

        Vector3 r = Vector3.Normalize(end - start);
        float d = Vector3.Distance(end, start);
        float R = d * d / 8.0f / h + h * 0.5f; // Rayon de courbure
        Vector3 perp = new Vector3(r.y, -r.x); // Vecteur perpendiculaire à start-end 

        Vector3 center = 0.5f * (start + end) + perp * (R - h); // Centre de la courbe

        float angle = -Vector3.Angle(start - center, end - center) / 180 * Mathf.PI;  // angle d'ouverture
        float angle0 = Vector3.Angle(start - center, Vector3.right) / 180 * Mathf.PI;  // angle de départ

        if (start.y - center.y < 0) angle0 = -angle0;  // Quelques corrections suivant l'orientation de la flèche
        if (R < h) angle = -2 * Mathf.PI - angle;


        int k = 0; // nombre de point (< NSample car on élimine ceux dans le collider)
  
        Vector3[] vectors = new Vector3[NSample+1];


        for (int i = 0; i < NSample + 1; i++)
        {
            Vector3 vec = center + new Vector3(Mathf.Cos(angle0 + angle / NSample * i) * R, Mathf.Sin(angle0 + angle / NSample * i) * R, -1);

            if (!colliderLiaison.OverlapPoint(vec) && !colliderAtome.OverlapPoint(vec)) 
                // On ne retient le point que s'il ne touche ni l'atome ni la liaison
            {
                vectors[k] = vec;
                k++;
            }
            
        }

        lr.positionCount = k-1;

        for (int i = 0; i < k-1 ; i++)
        {
            lr.SetPosition(i, vectors[i]);
        }

        head.SetPosition(0, vectors[k - 2]);
        head.SetPosition(1, vectors[k - 1]);


    }

}

