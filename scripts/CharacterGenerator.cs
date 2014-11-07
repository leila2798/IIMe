using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterGenerator : MonoBehaviour {

	// Creates a character based on the currentConfiguration recycling a
	// character base, this way the position and animation of the character
	// are not changed.
	public static GameObject Generate(GameObject root, string conf)
	{
		float startTime = Time.realtimeSinceStartup;
		
		// The SkinnedMeshRenderers that will make up a character will be
		// combined into one SkinnedMeshRenderers using multiple materials.
		// This will speed up rendering the resulting character.
		List<CombineInstance> combineInstances = new List<CombineInstance>();
		List<Material> materials = new List<Material>();
		List<Transform> bones = new List<Transform>();
		Transform[] transforms = root.GetComponentsInChildren<Transform>();



		/*
		foreach (CharacterElement element in currentConfiguration.Values)
		{
			SkinnedMeshRenderer smr = element.GetSkinnedMeshRenderer();
			materials.AddRange(smr.materials);
			for (int sub = 0; sub < smr.sharedMesh.subMeshCount; sub++)
			{
				CombineInstance ci = new CombineInstance();
				ci.mesh = smr.sharedMesh;
				ci.subMeshIndex = sub;
				combineInstances.Add(ci);
			}
			
			// As the SkinnedMeshRenders are stored in assetbundles that do not
			// contain their bones (those are stored in the characterbase assetbundles)
			// we need to collect references to the bones we are using
			foreach (string bone in element.GetBoneNames())
			{
				foreach (Transform transform in transforms)
				{
					if (transform.name != bone) continue;
					bones.Add(transform);
					break;
				}
			}
			
			Object.Destroy(smr.gameObject);
		}
		
		// Obtain and configure the SkinnedMeshRenderer attached to
		// the character base.
		SkinnedMeshRenderer r = root.GetComponent<SkinnedMeshRenderer>();
		r.sharedMesh = new Mesh();
		r.sharedMesh.CombineMeshes(combineInstances.ToArray(), false, false);
		r.bones = bones.ToArray();
		r.materials = materials.ToArray();
		*/
		Debug.Log("Generating character took: " + (Time.realtimeSinceStartup - startTime) * 1000 + " ms");
		return root;
	}

}
