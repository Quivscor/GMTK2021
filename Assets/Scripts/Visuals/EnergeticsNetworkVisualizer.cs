using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnergeticsNetworkVisualizer : MonoBehaviour
{
    public static readonly int INIT_LINERENDERER_COUNT = 50;
    public static readonly int INIT_PARTICLES_COUNT = 200;

    public List<LineRenderer> LineRenderers { get; private set; }
    public List<SpriteRenderer> SpriteRenderers { get; private set; }

    [SerializeField] private Transform m_RenderersParent;
    [SerializeField] private int m_SortingOrder;
    [SerializeField] private float m_LineWidth;
    [SerializeField] private Material m_LineMaterial;

    [SerializeField] private Sprite m_ParticleSprite;
    [SerializeField] private Vector3 m_ParticleScale;

    public Transform RenderersParent => m_RenderersParent;

    public List<EnergeticsNetwork> Networks { get; set; }
    private static List<Tuple<Vector3, Vector3>> MockConnections;

    private void Awake()
    {
        EnergeticsController.OnNetworksListChanged += UpdateNetworks;

        LineRenderers = new List<LineRenderer>();
        SpriteRenderers = new List<SpriteRenderer>();
        Networks = new List<EnergeticsNetwork>();
        MockConnections = new List<Tuple<Vector3, Vector3>>();

        for (int i = 0; i < INIT_LINERENDERER_COUNT; i++)
        {
            GameObject newGO = new GameObject("LineRenderer", typeof(LineRenderer));
            newGO.transform.parent = RenderersParent;

            LineRenderer lr = newGO.GetComponent<LineRenderer>();
            lr.material = new Material(lr.material);
            lr.sortingOrder = m_SortingOrder;
            lr.material = m_LineMaterial;
            lr.startWidth = lr.endWidth = m_LineWidth;
            LineRenderers.Add(lr);
        }

        for(int j = 0; j < INIT_PARTICLES_COUNT; j++)
        {
            GameObject newGO = new GameObject("EnergyParticle", typeof(SpriteRenderer));
            newGO.transform.parent = RenderersParent;

            SpriteRenderer sr = newGO.GetComponent<SpriteRenderer>();
            sr.sortingOrder = m_SortingOrder + 1;
            sr.sprite = m_ParticleSprite;
            sr.transform.localScale = m_ParticleScale;
            SpriteRenderers.Add(sr);
        }
    }

    public void UpdateNetworks(EnergeticsControllerEventData e)
    {
        Networks = e.networks;
    }

    public static void UpdateMockConnections(Vector3 mockPosition)
    {
        ClearMockConnections();

        Collider2D[] cols = Physics2D.OverlapCircleAll(mockPosition, EnergeticsController.ConnectionDistance);
        for (int i = 0; i < cols.Length; i++)
        {
            IEnergetics node;
            //if yes, add connections
            if (cols[i].TryGetComponent<IEnergetics>(out node))
            {
                MockConnections.Add(new Tuple<Vector3, Vector3>(mockPosition, node.TransformReference.position));
            }
        }
    }

    public static void ClearMockConnections()
    {
        MockConnections.Clear();
    }

    private void LateUpdate()
    {
        int networkConnectionsCount = 0;
        int particleCount = 0;
        foreach(EnergeticsNetwork network in Networks)
        {
            DrawNetworkLines(CreatePairList(network), ref networkConnectionsCount, Color.white);
            DrawNetworkParticles(network, ref particleCount);
        }
        //draw connections to mock if there are any
        DrawNetworkLines(MockConnections, ref networkConnectionsCount, Color.green);

        HideRemainingNetworkLines(networkConnectionsCount);
        HideRemainingParticles(particleCount);
    }

    private void DrawNetworkLines(List<Tuple<Vector3, Vector3>> nodePairs, ref int startIndex, Color color)
    {
        for(int i = startIndex; i < startIndex + nodePairs.Count; i++)
        {
            LineRenderers[i].material.color = color;
            LineRenderers[i].gameObject.SetActive(true);
            LineRenderers[i].SetPositions(new Vector3[] {nodePairs[i - startIndex].Item1,
                nodePairs[i - startIndex].Item2});
        }
        startIndex += nodePairs.Count;
    }

    private List<Tuple<Vector3, Vector3>> CreatePairList(EnergeticsNetwork network)
    {
        List<Tuple<Vector3, Vector3>> nodePairs = new List<Tuple<Vector3, Vector3>>();
        foreach (IPathfindingNode node in network.Nodes)
        {
            if (node.NetworkNeighbours.Count == 0)
                continue;

            foreach (IPathfindingNode neighbour in node.NetworkNeighbours)
            {
                if (nodePairs.Contains(Tuple.Create(neighbour.TransformReference.position, node.TransformReference.position)))
                    continue;

                if ((!node.IsWalkable || !neighbour.IsWalkable))
                    continue;

                nodePairs.Add(Tuple.Create(node.TransformReference.position, neighbour.TransformReference.position));
            }
        }
        return nodePairs;
    }

    private void HideRemainingNetworkLines(int startIndex)
    {
        for (int j = startIndex; j < LineRenderers.Count; j++)
        {
            LineRenderers[j].gameObject.SetActive(false);
        }
    }

    private void DrawNetworkParticles(EnergeticsNetwork network, ref int startIndex)
    {
        List<EnergyParticle> particles = new List<EnergyParticle>(network.Particles);
        
        for(int i = startIndex; i < startIndex + particles.Count; i++)
        {
            SpriteRenderers[i].gameObject.SetActive(true);
            SpriteRenderers[i].transform.position = particles[i - startIndex].Position;
        }
        startIndex += particles.Count;
    }    

    private void HideRemainingParticles(int startIndex)
    {
        for (int j = startIndex; j < INIT_PARTICLES_COUNT; j++)
        {
            SpriteRenderers[j].gameObject.SetActive(false);
        }
    }
}
