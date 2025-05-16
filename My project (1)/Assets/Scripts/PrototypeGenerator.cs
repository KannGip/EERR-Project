using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Windows;
public class PrototypeGenerator : MonoBehaviour
{
    public List<Prototype> protoypePrefabs;
    public List<Prototype> prototypes;
    public List<Prototype> specialStartPrefabs;
    public List<Prototype> specialEndPrefabs;
    public string path = "Assets/Data/Prototypes";
    WFC_Socket posXHolder;
    WFC_Socket negXHolder;
    WFC_Socket posZHolder;
    WFC_Socket negZHolder;
    List<GameObject> prototypeHolder = new List<GameObject>();

    public List<Prototype> startPrototypes = new List<Prototype>();
    public List<Prototype> endPrototypes = new List<Prototype>();

    [SerializeField] private GameObject prototypeHolderPrefab;
    
    [ContextMenu("Generate Prototypes")]
    public void GeneratePrototypes()
    {
        startPrototypes.Clear();
        endPrototypes.Clear();
        prototypes.Clear();
        #if UNITY_EDITOR
        //if (Directory.Exists(path)) 
        //    Directory.Delete(path);

        Directory.CreateDirectory(path);
        #endif

        // Generate rotations for all prototypes
        for (int i = 0; i < protoypePrefabs.Count; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Prototype newProto = CreateMyAsset(path, protoypePrefabs[i].name, j.ToString().Replace(" ", ""));

                newProto.prefab = protoypePrefabs[i].prefab;
                newProto.isWalkable = protoypePrefabs[i].isWalkable;
                RotatePrototypeSockets(newProto, protoypePrefabs[i], j);
                newProto.directions = CalculateDirectionsFromSockets(newProto);

                
                prototypes.Add(newProto);
               
            }
        }

        for (int i = 0; i < specialStartPrefabs.Count; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Prototype p = CreateMyAsset(path, "Start_" + specialStartPrefabs[i].name, j.ToString());
                p.prefab = specialStartPrefabs[i].prefab;
                p.meshRotation = j;
                p.attributes = specialStartPrefabs[i].attributes;
                CopyAndRotateSockets(p, specialStartPrefabs[i], j);
                p.directions = CalculateDirectionsFromSockets(p);
                startPrototypes.Add(ScriptableObject.Instantiate(p));
            }
        }

        for (int i = 0; i < specialEndPrefabs.Count; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Prototype p = CreateMyAsset(path, "End_" + specialEndPrefabs[i].name, j.ToString());
                p.prefab = specialEndPrefabs[i].prefab;
                p.meshRotation = j;
                p.attributes = specialEndPrefabs[i].attributes;
                CopyAndRotateSockets(p, specialEndPrefabs[i], j);
                p.directions = CalculateDirectionsFromSockets(p);
                endPrototypes.Add(ScriptableObject.Instantiate(p));
            }
        }
        UpdatePrototypes();

        prototypeHolderPrefab.GetComponent<Cell>().possiblePrototypes = prototypes;

       

        #if UNITY_EDITOR
        PrefabUtility.SaveAsPrefabAsset(prototypeHolderPrefab, "Assets/Prefabs/cell_holder.prefab");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        #endif

    }
    public void UpdatePrototypes()
    {
        // Generate rotations for all prototypes
        for (int i = 0; i < protoypePrefabs.Count; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                prototypes[i*4+j].prefab = protoypePrefabs[i].prefab;
                prototypes[i*4+j].validNeighbours = new NeighbourList();
                prototypes[i*4+j].meshRotation = j;
                prototypes[i*4+j].attributes = protoypePrefabs[i].attributes;

                prototypes[i*4+j].posX = protoypePrefabs[i].posX;
                prototypes[i*4+j].negX = protoypePrefabs[i].negX;
                prototypes[i*4+j].posZ = protoypePrefabs[i].posZ;
                prototypes[i*4+j].negZ = protoypePrefabs[i].negZ;

                if(j==0)
                {
                    posXHolder = prototypes[i*4+j].posX;
                    negXHolder = prototypes[i*4+j].negX;
                    posZHolder = prototypes[i*4+j].posZ;
                    negZHolder = prototypes[i*4+j].negZ;
                }
                else
                {
                    prototypes[i*4+j].negZ = posXHolder;
                    prototypes[i*4+j].negX = negZHolder;
                    prototypes[i*4+j].posZ = negXHolder;
                    prototypes[i*4+j].posX = posZHolder;

                    posXHolder = prototypes[i*4+j].posX;
                    negXHolder = prototypes[i*4+j].negX;
                    posZHolder = prototypes[i*4+j].posZ;
                    negZHolder = prototypes[i*4+j].negZ;
                }
            }
        }

        // // Generate valid neighbors
        for (int i = 0; i < prototypes.Count; i++)
            prototypes[i].validNeighbours = GetValidNeighbors(prototypes[i]);
    }

    private void CopyAndRotateSockets(Prototype target, Prototype source, int rotation)
    {
        // Copy base sockets
        WFC_Socket px = source.posX;
        WFC_Socket nx = source.negX;
        WFC_Socket pz = source.posZ;
        WFC_Socket nz = source.negZ;

        // Rotate based on rotation index
        switch (rotation % 4)
        {
            case 0:
                target.posX = px;
                target.negX = nx;
                target.posZ = pz;
                target.negZ = nz;
                break;
            case 1:
                target.posX = nz;
                target.negX = pz;
                target.posZ = px;
                target.negZ = nx;
                break;
            case 2:
                target.posX = nx;
                target.negX = px;
                target.posZ = nz;
                target.negZ = pz;
                break;
            case 3:
                target.posX = pz;
                target.negX = nz;
                target.posZ = nx;
                target.negZ = px;
                break;
        }
    }

    public static Prototype CreateMyAsset(string assetFolder, string name, string j)
    {
        Prototype asset = ScriptableObject.CreateInstance<Prototype>();
        #if UNITY_EDITOR
            AssetDatabase.CreateAsset(asset, assetFolder+"/"+name+"_"+j+".asset");
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
        #endif

        return asset;
    }
    private NeighbourList GetValidNeighbors(Prototype proto)
    {
        NeighbourList neighbourList = new NeighbourList();
        foreach(Prototype p in prototypes)
        {
            if(proto.posX==p.negX)
                neighbourList.posX.Add(p);
            if(proto.negX==p.posX)
                neighbourList.negX.Add(p);
            if(proto.posZ==p.negZ)
                neighbourList.posZ.Add(p);
            if(proto.negZ==p.posZ)
                neighbourList.negZ.Add(p);
        }
        return neighbourList;
    }
    public void DisplayPrototypes()
    {
        if(prototypeHolder.Count!=0)
        {
            foreach(GameObject p in prototypeHolder)
                DestroyImmediate(p);

            prototypeHolder = new List<GameObject>();
        }

        for (int i = 0; i < protoypePrefabs.Count; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                GameObject protoObj = Instantiate(protoypePrefabs[i].prefab, new Vector3(i*1.5f, 0f, j*1.5f), Quaternion.identity, this.transform);
                protoObj.transform.Rotate(new Vector3(0f, j*90, 0f), Space.Self);
                protoObj.name = (protoypePrefabs[i].prefab.name +"_"+j.ToString());
                prototypeHolder.Add(protoObj);
            }
        }
    }

/*
    private DirectionFlags RotateDirections(DirectionFlags original, int rotation)
    {
        DirectionFlags result = original;
        for (int i = 0; i < rotation; i++)
        {
            DirectionFlags temp = 0;
            if (result.HasFlag(DirectionFlags.PosX)) temp |= DirectionFlags.NegZ;
            if (result.HasFlag(DirectionFlags.NegZ)) temp |= DirectionFlags.NegX;
            if (result.HasFlag(DirectionFlags.NegX)) temp |= DirectionFlags.PosZ;
            if (result.HasFlag(DirectionFlags.PosZ)) temp |= DirectionFlags.PosX;
            result = temp;
        }
        return result;
    }
*/

    private void RotatePrototypeSockets(Prototype target, Prototype original, int rotation)
    {
        WFC_Socket px = original.posX;
        WFC_Socket nx = original.negX;
        WFC_Socket pz = original.posZ;
        WFC_Socket nz = original.negZ;

        switch (rotation % 4)
        {
            case 0:
                target.posX = px; target.negX = nx; target.posZ = pz; target.negZ = nz;
                break;
            case 1:
                target.posX = nz; target.negX = pz; target.posZ = px; target.negZ = nx;
                break;
            case 2:
                target.posX = nx; target.negX = px; target.posZ = nz; target.negZ = pz;
                break;
            case 3:
                target.posX = pz; target.negX = nz; target.posZ = nx; target.negZ = px;
                break;
        }
    }

    private DirectionFlags CalculateDirectionsFromSockets(Prototype proto)
    {
        DirectionFlags flags = DirectionFlags.None;
        if (proto.posX != WFC_Socket.None) flags |= DirectionFlags.PosX;
        if (proto.negX != WFC_Socket.None) flags |= DirectionFlags.NegX;
        if (proto.posZ != WFC_Socket.None) flags |= DirectionFlags.PosZ;
        if (proto.negZ != WFC_Socket.None) flags |= DirectionFlags.NegZ;
        return flags;
    }

}
