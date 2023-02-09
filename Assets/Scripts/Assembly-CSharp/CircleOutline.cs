using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleOutline : BaseMeshEffect
{
	[SerializeField]
	private Color m_EffectColor = new Color(0f, 0f, 0f, 0.5f);

	[SerializeField]
	private float m_EffectDistance = 1f;

	[SerializeField]
	private int m_nEffectNumber = 4;

	[SerializeField]
	private bool m_UseGraphicAlpha = true;

	public override void ModifyMesh(VertexHelper vh)
	{
		if (IsActive())
		{
			List<UIVertex> list = new List<UIVertex>();
			vh.GetUIVertexStream(list);
			ModifyVertices(list);
			vh.Clear();
			vh.AddUIVertexTriangleStream(list);
		}
	}

	private void ModifyVertices(List<UIVertex> verts)
	{
		int start = 0;
		int count = verts.Count;
		for (int i = 0; i < m_nEffectNumber; i++)
		{
			float f = (float)Math.PI * 2f * (float)i / (float)m_nEffectNumber;
			float x = m_EffectDistance * Mathf.Cos(f);
			float y = m_EffectDistance * Mathf.Sin(f);
			ApplyShadow(verts, start, count, x, y);
			start = count;
			count = verts.Count;
		}
	}

	private void ApplyShadow(List<UIVertex> verts, int start, int end, float x, float y)
	{
		for (int i = start; i < end; i++)
		{
			UIVertex uIVertex = verts[i];
			verts.Add(uIVertex);
			Vector3 position = uIVertex.position;
			position.x += x;
			position.y += y;
			uIVertex.position = position;
			Color32 color = m_EffectColor;
			if (m_UseGraphicAlpha)
			{
				color.a = (byte)((float)(color.a * verts[i].color.a) / 255f);
			}
			uIVertex.color = color;
			verts[i] = uIVertex;
		}
	}
}
