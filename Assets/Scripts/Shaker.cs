using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Shaker : MonoBehaviour
{
    public static Shaker instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public IEnumerator ShakeImage(Image image)
    {   
        yield return new WaitForSeconds(1.0f);

        float x = image.rectTransform.anchoredPosition.x, y = image.rectTransform.anchoredPosition.y;

        while (true)
        {
            if (image == null)
            {
                break;
            }

            image.rectTransform.DOAnchorPos(new Vector3(x + 0.75f, y, 0.0f), 0.03f);
            yield return new WaitForSeconds(0.03f);
            image.rectTransform.DOAnchorPos(new Vector3(x, y + 0.75f, 0.0f), 0.03f);
            yield return new WaitForSeconds(0.03f);
            image.rectTransform.DOAnchorPos(new Vector3(x, y - 0.75f, 0.0f), 0.03f);
            yield return new WaitForSeconds(0.03f);
            image.rectTransform.DOAnchorPos(new Vector3(x - 0.75f, y, 0.0f), 0.03f);
            yield return new WaitForSeconds(0.03f);
        }
    }

    public IEnumerator SwingImage(Image image)
    {
        yield return new WaitForSeconds(1.0f);

        image.rectTransform.pivot = new Vector2(0.5f, 1.0f);
        image.rectTransform.anchoredPosition += new Vector2(0.0f, 75.0f);

        while (true)
        {
            if (image == null)
            {
                break;
            }

            image.rectTransform.DORotate(new Vector3(0.0f, 0.0f, 10.0f), 0.5f);
            yield return new WaitForSeconds(0.5f);
            image.rectTransform.DORotate(new Vector3(0.0f, 0.0f, -10.0f), 0.5f);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public IEnumerator ShakeText(TextMeshProUGUI textMesh, float delay) {
        yield return new WaitForSeconds(delay);

        textMesh.ForceMeshUpdate();

        TMP_TextInfo textInfo = textMesh.textInfo;
        Vector3[][] copyOfVertices = new Vector3[0][];
        bool hasTextChanged = true;

        TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();

        Vector3[] cachedVertexInfo = cachedMeshInfo[textMesh.textInfo.characterInfo[0].materialReferenceIndex].vertices;

        while (true) {
            if (Time.time == delay + 36.0f || textMesh == null)
            {
                break;
            }

            for (int index = 0; index < textMesh.text.Length; index++) {
                if (hasTextChanged)
                {
                    if (copyOfVertices.Length < textInfo.meshInfo.Length)
                        copyOfVertices = new Vector3[textInfo.meshInfo.Length][];
                    

                    for (int i = 0; i < textInfo.meshInfo.Length; i++)
                    {
                        int length = textInfo.meshInfo[i].vertices.Length;
                        copyOfVertices[i] = new Vector3[length];
                    }

                    hasTextChanged = false;
                }

                if (!textMesh.textInfo.characterInfo[index].isVisible) {
                    continue; // dont need to shake it if its not visible!
                }

                float modX = Random.Range(-3, 3);
                float modY = Random.Range(-3, 3);

                Vector3 modifier = new Vector3(modX, modY, 0f);
                int materialIndex = textMesh.textInfo.characterInfo[index].materialReferenceIndex;
                Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;
                int vertexIndex = textInfo.characterInfo[index].vertexIndex;

                for (int i = 0; i < textMesh.textInfo.meshInfo.Length; i++) {
                    copyOfVertices[materialIndex] = textMesh.textInfo.meshInfo[i].mesh.vertices;
                }

                copyOfVertices[materialIndex][vertexIndex + 0] = sourceVertices[vertexIndex + 0] + modifier;
                copyOfVertices[materialIndex][vertexIndex + 1] = sourceVertices[vertexIndex + 1] + modifier;
                copyOfVertices[materialIndex][vertexIndex + 2] = sourceVertices[vertexIndex + 2] + modifier;
                copyOfVertices[materialIndex][vertexIndex + 3] = sourceVertices[vertexIndex + 3] + modifier;

                for (int i = 0; i < textMesh.textInfo.meshInfo.Length; i++) {
                    textMesh.textInfo.meshInfo[i].mesh.vertices = copyOfVertices[i];
                    textMesh.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
}
