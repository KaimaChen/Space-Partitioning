using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KdTreeDemo : MonoBehaviour
{
    private static Material mMat;
    KdTree mTree = new KdTree();

    // Use this for initialization
    void Start() {
        mMat = new Material(Shader.Find("Unlit/Color"));

        mTree.Insert(new int[] { 200, 200 });
        mTree.Insert(new int[] { 50, 25 });
        mTree.Insert(new int[] { 250, 250 });
        mTree.Insert(new int[] { 150, 170 });
        mTree.Insert(new int[] { 300, 100 });
        mTree.Insert(new int[] { 100, 40 });
        
        mTree.Print();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnGUI()
    {
        mTree.ShowGraph(mMat);
    }
}
