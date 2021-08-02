using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

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

    private void Awake()
    {
        EnergeticsController.OnNetworksListChanged += UpdateNetworks;

        LineRenderers = new List<LineRenderer>();
        SpriteRenderers = new List<SpriteRenderer>();
        Networks = new List<EnergeticsNetwork>();

        for(int i = 0; i < INIT_LINERENDERER_COUNT; i++)
        {
            GameObject newGO = new GameObject("LineRenderer", typeof(LineRenderer));
            newGO.transform.parent = RenderersParent;

            LineRenderer lr = newGO.GetComponent<LineRenderer>();
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

    private void LateUpdate()
    {
        int networkConnectionsCount = 0;
        int particleCount = 0;
        foreach(EnergeticsNetwork network in Networks)
        {
            DrawNetworkLines(network, ref networkConnectionsCount);
            DrawNetworkParticles(network, ref particleCount);
        }
        HideRemainingNetworkLines(networkConnectionsCount);
        HideRemainingParticles(particleCount);
    }

    private void DrawNetworkLines(EnergeticsNetwork network, ref int startIndex)
    {
        List<Tuple<IPathfindingNode, IPathfindingNode>> nodePairs = CreatePairList(network);

        for(int i = startIndex; i < startIndex + nodePairs.Count; i++)
        {
            LineRenderers[i].positionCount = 2;
            LineRenderers[i].SetPositions(new Vector3[] {nodePairs[i - startIndex].Item1.TransformReference.position,
                nodePairs[i - startIndex].Item2.TransformReference.position});
        }
        startIndex += nodePairs.Count;
    }

    private List<Tuple<IPathfindingNode, IPathfindingNode>> CreatePairList(EnergeticsNetwork network)
    {
        List<Tuple<IPathfindingNode, IPathfindingNode>> nodePairs = new List<Tuple<IPathfindingNode, IPathfindingNode>>();
        foreach (IPathfindingNode node in network.Nodes)
        {
            if (node.NetworkNeighbours.Count == 0)
                continue;

            foreach (IPathfindingNode neighbour in node.NetworkNeighbours)
            {
                if (nodePairs.Contains(Tuple.Create(neighbour, node)))
                    continue;

                if ((!node.IsWalkable && !neighbour.IsWalkable))
                    continue;

                nodePairs.Add(Tuple.Create(node, neighbour));
            }
        }
        return nodePairs;
    }

    private void HideRemainingNetworkLines(int startIndex)
    {
        for (int j = startIndex; j < LineRenderers.Count; j++)
        {
            LineRenderers[j].positionCount = 0;
        }
    }

    private void DrawNetworkParticles(EnergeticsNetwork network, ref int startIndex)
    {
        List<EnergyParticle> particles = new List<EnergyParticle>(network.Particles);
        
        for(int i = startIndex; i < startIndex + particles.Count; i++)
        {
            SpriteRenderers[i].transform.position = particles[i - startIndex].Position;
            SpriteRenderers[i].color = Color.white;
        }
        startIndex += particles.Count;
    }    

    private void HideRemainingParticles(int startIndex)
    {
        for (int j = startIndex; j < INIT_PARTICLES_COUNT; j++)
        {
            SpriteRenderers[j].color = Color.clear;
        }
    }
}
