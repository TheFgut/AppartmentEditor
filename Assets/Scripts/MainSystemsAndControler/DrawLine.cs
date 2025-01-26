using UnityEngine;
using HelpersFunctions;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class DrawLine : MonoBehaviour, IActivatable
{
    [SerializeField] private float distanceToGenPoint = 1;
    [SerializeField] private GameObject circlePrefab;
    [SerializeField] private Material lineMaterial;
    [SerializeField] private int sortingLayerID = 0;

    private bool activated;

    private LineRenderer lineRenderer;
    private GameObject currentCircle;
    private bool isDrawing = false;
    private Vector3 startPosition;
    private bool drawingStraigth;
    //straigth line timer
    private Vector3 straighLineDir;
    private float straigthLineDirGetTimer;
    private bool straightLineDirTimerStarted;
    private const float straigthLineDirGetTime = 0.1f;
    public Action<Vector3[]> onLineDrawed {  get; set; }

    #region activation

    public void Activate()
    {
        activated = true;
    }

    public void Deactivate()
    {
        activated = false;
    }

    #endregion

    #region drawing
    void Update()
    {
        if (!activated)
        {
            if (isDrawing)
            {
                StopDrawing();
            }
            return;
        }
        //start of the line
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                straigthLineDirGetTimer = 0;
                straightLineDirTimerStarted = true;
            }
            Vector3 mousePosition = GetMouseWorldPosition();
            if (!Helpers.isVectorFinite(mousePosition)) return;
            StartDrawing(mousePosition);
        }

        //line drawing
        if (isDrawing && Input.GetMouseButton(0))
        {
            Vector3 mousePosition = GetMouseWorldPosition();

            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                straigthLineDirGetTimer = 0;
                straightLineDirTimerStarted = true;
            }
            if (straightLineDirTimerStarted)
            {
                if(straigthLineDirGetTimer < straigthLineDirGetTime)
                {
                    straighLineDir = mousePosition - startPosition;
                    straigthLineDirGetTimer += Time.deltaTime;
                }
                else
                {
                    straightLineDirTimerStarted = false;
                }
            }

            if (!Helpers.isVectorFinite(mousePosition)) return;
            UpdateDrawing(mousePosition);

            //checking if line closed
            if(lineRenderer.positionCount > 2)
            {
                float distBetweenFirstAndLAst = (lineRenderer.GetPosition(0) -
                    lineRenderer.GetPosition(lineRenderer.positionCount - 1)).magnitude;
                if (distBetweenFirstAndLAst < Helpers.distanceToLinkLines)
                {
                    StopDrawing();
                }
            }
        }

        //ending line
        if (Input.GetMouseButtonUp(0) && isDrawing)
        {
            StopDrawing();
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Selected")))
        {
            Vector3 hitPosition = hit.point;
            return hitPosition;
        }
        return Vector3.negativeInfinity;
    }

    private void StartDrawing(Vector3 position)
    {
        startPosition = position;

        currentCircle = Instantiate(circlePrefab, position, Quaternion.identity);

        GameObject lineObject = new GameObject("Line");
        lineRenderer = lineObject.AddComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, position);
        lineRenderer.SetPosition(1, position);

        lineRenderer.sortingOrder = sortingLayerID;

        isDrawing = true;
    }

    private void UpdateDrawing(Vector3 position)
    {
        //drawing straigth line
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            drawingStraigth = true;
            Vector3 targetPos = Helpers.ProjectPointOnLine(position, startPosition, straighLineDir);
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, targetPos);
            return;
        }
        //drawing curve
        if(!drawingStraigth) lineRenderer.SetPosition(lineRenderer.positionCount - 1, position);
        if (((lineRenderer.GetPosition(lineRenderer.positionCount - 2) - position).magnitude) > distanceToGenPoint)
        {
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, position);
        }
        drawingStraigth = false;
    }

    private void StopDrawing()
    {
        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < lineRenderer.positionCount;i++)
        {
            positions.Add(lineRenderer.GetPosition(i));
        }
        Destroy(currentCircle);
        Destroy(lineRenderer.gameObject);
        isDrawing = false;
        onLineDrawed?.Invoke(positions.ToArray());
    }
    #endregion
}
