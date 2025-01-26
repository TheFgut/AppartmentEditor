using EzySlice;
using HelpersFunctions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using Zenject;

public class GlobalStateController : MonoBehaviour
{
    [SerializeField] private DrawLine lineDrawer;
    [SerializeField] private WallSelectorController wallSelectorController;

    private DiContainer container;
    private ModalInfoWindow infoModal;
    public GlobalGameState activeState {  get; private set; }
    public Action<GlobalGameState> onStateChanges {  get; set; }

    [Inject]
    protected void Construct(DiContainer container, ModalInfoWindow infoModal)
    {
        this.container = container;
        this.infoModal = infoModal;
    }


    private void Start()
    {
        ChangeState(GlobalGameState.Selection);
    }

    public void ChangeState(GlobalGameState newState)
    {
        if (activeState == newState) return;
        activeState = newState;

        switch (activeState)
        {
            case GlobalGameState.Selection:
                lineDrawer.onLineDrawed -= SliceObject;
                lineDrawer.Deactivate();
                wallSelectorController.Activate();
                break;
            case GlobalGameState.Drawing:
                lineDrawer.onLineDrawed += SliceObject;
                lineDrawer.Activate();
                wallSelectorController.Deactivate();
                break;
        }

        onStateChanges?.Invoke(newState);
    }

    //TODO:move this logic to class WallSlicer
    #region slicing
    public void SliceObject(Vector3[] linePoints)
    {
        if(wallSelectorController.selected != null)
        {
            EditableWall wall = wallSelectorController.selected;
            wall.Deselect();
            bool success = CutObjectWithLine(linePoints, wall);
            if (!success) wall.Select();
        }
    }

    public bool CutObjectWithLine(Vector3[] linePoints, EditableWall wall)
    {
        if (linePoints == null || linePoints.Length < 2)
        {
            //fail
            infoModal.OpenForError("Error", "Poits to slice is null or less than 2");
            return false;
        }
        else if (linePoints.Length == 2)
        {
            SliceOnHalf(linePoints, wall);
            return true;
        }
        else
        {
            if (!Helpers.isVectorsSame(linePoints[0], linePoints[linePoints.Length - 1], Helpers.distanceToLinkLines + 0.01f))
            {
                //not supported message but success
                infoModal.OpenForError("Fail", "You need to close the line for the cut to occur");
                return false;
            }
            else
            {
                //success but nothing happens
                infoModal.Open("Success", "You've done all correct but this action is currently not awailable");
                return true;
            }
        }

        //GameObject targetObject = gameObject;
        //for (int lineNum = 0; lineNum < linePoints.Length - 1; lineNum++)
        //{
        //    EzySlice.Plane slicingPlane = new EzySlice.Plane(linePoints[lineNum], linePoints[lineNum] - linePoints[lineNum + 1]);

        //    SlicedHull slicedHull = targetObject.Slice(slicingPlane);

        //    if (slicedHull != null)
        //    {
        //        GameObject upperHull = slicedHull.CreateUpperHull(targetObject);
        //        upperHull.transform.position = targetObject.transform.position;
        //        upperHull.transform.rotation = targetObject.transform.rotation;

        //        GameObject lowerHull = slicedHull.CreateLowerHull(targetObject);
        //        lowerHull.transform.position = targetObject.transform.position;
        //        lowerHull.transform.rotation = targetObject.transform.rotation;

        //        GameObject combinedObject = new GameObject("CombinedObject");
        //        MeshFilter combinedMeshFilter = combinedObject.AddComponent<MeshFilter>();
        //        MeshRenderer combinedRenderer = combinedObject.AddComponent<MeshRenderer>();
        //        Mesh combinedMesh = CombineMeshes(upperHull, lowerHull);
        //        combinedMeshFilter.mesh = combinedMesh;
        //        //combinedRenderer.material = material;

        //        combinedObject.transform.position = targetObject.transform.position;
        //        combinedObject.transform.rotation = targetObject.transform.rotation;

        //        GameObject.Destroy(targetObject);
        //    }

        //}
    }

    private void SliceOnHalf(Vector3[] points, EditableWall wall)
    {
        GameObject wallGameobject = wall.gameObject;

        Vector3 sliceLine = points[0] - points[1];
        Quaternion rotation = Quaternion.AngleAxis(90, sliceLine);
        EzySlice.Plane slicingPlane = new EzySlice.Plane(points[0], rotation * wall.config.normal);

        SlicedHull slicedHull = wallGameobject.Slice(slicingPlane);

        if (slicedHull != null)
        {
            GameObject upperHull = slicedHull.CreateUpperHull(wallGameobject);
            CreateWallFromGameobject(upperHull, wallGameobject, wall.config);

            GameObject lowerHull = slicedHull.CreateLowerHull(wallGameobject);
            CreateWallFromGameobject(lowerHull, wallGameobject, wall.config);

            Destroy(wall.gameObject);
        }
    }

    private void CreateWallFromGameobject(GameObject obj, GameObject parent, WallConfig config)
    {
        obj.transform.position = parent.transform.position;
        obj.transform.rotation = parent.transform.rotation;
        EditableWall wallScr = obj.AddComponent<EditableWall>();
        wallScr.Configure(config);
        ObjectClick clickScr = obj.AddComponent<ObjectClick>();
        MeshCollider colider = obj.AddComponent<MeshCollider>();
        container.Inject(wallScr);
        container.Inject(clickScr);
    }


    private Mesh CombineMeshes(GameObject upperHull, GameObject lowerHull)
    {
        MeshFilter upperMeshFilter = upperHull.GetComponent<MeshFilter>();
        MeshFilter lowerMeshFilter = lowerHull.GetComponent<MeshFilter>();

        Mesh combinedMesh = new Mesh();

        CombineInstance[] combine = new CombineInstance[2];

        combine[0].mesh = upperMeshFilter.mesh;
        combine[0].transform = upperHull.transform.localToWorldMatrix;

        combine[1].mesh = lowerMeshFilter.mesh;
        combine[1].transform = lowerHull.transform.localToWorldMatrix;

        combinedMesh.CombineMeshes(combine, true, true);

        return combinedMesh;
    }
    #endregion
}

public enum GlobalGameState
{
    None,
    Selection,
    Drawing,
    Modifying
}