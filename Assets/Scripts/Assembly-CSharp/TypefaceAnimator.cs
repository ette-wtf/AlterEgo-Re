using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
[AddComponentMenu("UI/Effects/TypefaceAnimator")]
public class TypefaceAnimator : BaseMeshEffect
{
	public enum TimeMode
	{
		Time = 0,
		Speed = 1
	}

	public enum Style
	{
		Once = 0,
		Loop = 1,
		PingPong = 2
	}

	public TimeMode timeMode;

	public float duration = 1f;

	public float speed = 5f;

	public float delay;

	public Style style;

	public bool playOnAwake = true;

	[SerializeField]
	private float m_progress = 1f;

	public bool usePosition;

	public bool useRotation;

	public bool useScale;

	public bool useAlpha;

	public bool useColor;

	public UnityEvent onStart;

	public UnityEvent onComplete;

	[SerializeField]
	private int characterNumber;

	private float animationTime;

	private Coroutine playCoroutine;

	private bool m_isPlaying;

	public Vector3 positionFrom = Vector3.zero;

	public Vector3 positionTo = Vector3.zero;

	public AnimationCurve positionAnimationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public float positionSeparation = 0.5f;

	public float rotationFrom;

	public float rotationTo;

	public Vector2 rotationPivot = new Vector2(0.5f, 0.5f);

	public AnimationCurve rotationAnimationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public float rotationSeparation = 0.5f;

	public bool scaleSyncXY = true;

	public float scaleFrom;

	public float scaleTo = 1f;

	public Vector2 scalePivot = new Vector2(0.5f, 0.5f);

	public AnimationCurve scaleAnimationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public float scaleFromY;

	public float scaleToY = 1f;

	public Vector2 scalePivotY = new Vector2(0.5f, 0.5f);

	public AnimationCurve scaleAnimationCurveY = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public float scaleSeparation = 0.5f;

	public float alphaFrom;

	public float alphaTo = 1f;

	public AnimationCurve alphaAnimationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public float alphaSeparation = 0.5f;

	public Color colorFrom = Color.white;

	public Color colorTo = Color.white;

	public AnimationCurve colorAnimationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public float colorSeparation = 0.5f;

	public float progress
	{
		get
		{
			return m_progress;
		}
		set
		{
			m_progress = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	public bool isPlaying
	{
		get
		{
			return m_isPlaying;
		}
	}

	protected override void OnEnable()
	{
		if (playOnAwake)
		{
			Play();
		}
		base.OnEnable();
	}

	protected override void OnDisable()
	{
		Stop();
		base.OnDisable();
	}

	public void Play()
	{
		progress = 0f;
		switch (timeMode)
		{
		case TimeMode.Time:
			animationTime = duration;
			break;
		case TimeMode.Speed:
			animationTime = (float)characterNumber / 10f / speed;
			break;
		}
		switch (style)
		{
		case Style.Once:
			playCoroutine = StartCoroutine(PlayOnceCoroutine());
			break;
		case Style.Loop:
			playCoroutine = StartCoroutine(PlayLoopCoroutine());
			break;
		case Style.PingPong:
			playCoroutine = StartCoroutine(PlayPingPongCoroutine());
			break;
		}
	}

	public void Stop()
	{
		if (playCoroutine != null)
		{
			StopCoroutine(playCoroutine);
		}
		m_isPlaying = false;
		playCoroutine = null;
	}

	private IEnumerator PlayOnceCoroutine()
	{
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}
		if (!m_isPlaying)
		{
			m_isPlaying = true;
			if (onStart != null)
			{
				onStart.Invoke();
			}
			while (progress < 1f)
			{
				progress += Time.deltaTime / animationTime;
				yield return null;
			}
			m_isPlaying = false;
			progress = 1f;
			if (onComplete != null)
			{
				onComplete.Invoke();
			}
		}
	}

	private IEnumerator PlayLoopCoroutine()
	{
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}
		if (m_isPlaying)
		{
			yield break;
		}
		m_isPlaying = true;
		if (onStart != null)
		{
			onStart.Invoke();
		}
		while (true)
		{
			progress += Time.deltaTime / animationTime;
			if (progress > 1f)
			{
				progress -= 1f;
			}
			yield return null;
		}
	}

	private IEnumerator PlayPingPongCoroutine()
	{
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}
		if (m_isPlaying)
		{
			yield break;
		}
		m_isPlaying = true;
		if (onStart != null)
		{
			onStart.Invoke();
		}
		bool isPositive = true;
		while (true)
		{
			float num = Time.deltaTime / animationTime;
			if (isPositive)
			{
				progress += num;
				if (progress > 1f)
				{
					isPositive = false;
					progress -= num;
				}
			}
			else
			{
				progress -= num;
				if (progress < 0f)
				{
					isPositive = true;
					progress += num;
				}
			}
			yield return null;
		}
	}

	public override void ModifyMesh(VertexHelper vertexHelper)
	{
		if (!IsActive() || vertexHelper.currentVertCount == 0)
		{
			return;
		}
		List<UIVertex> list = new List<UIVertex>();
		vertexHelper.GetUIVertexStream(list);
		List<UIVertex> list2 = new List<UIVertex>();
		for (int i = 0; i < list.Count; i++)
		{
			int num = i % 6;
			if (num == 0 || num == 1 || num == 2 || num == 4)
			{
				list2.Add(list[i]);
			}
		}
		ModifyVertices(list2);
		List<UIVertex> list3 = new List<UIVertex>(list.Count);
		for (int j = 0; j < list.Count / 6; j++)
		{
			int num2 = j * 4;
			list3.Add(list2[num2]);
			list3.Add(list2[num2 + 1]);
			list3.Add(list2[num2 + 2]);
			list3.Add(list2[num2 + 2]);
			list3.Add(list2[num2 + 3]);
			list3.Add(list2[num2]);
		}
		vertexHelper.Clear();
		vertexHelper.AddUIVertexTriangleStream(list3);
	}

	public void ModifyVertices(List<UIVertex> verts)
	{
		if (IsActive())
		{
			Modify(verts);
		}
	}

	private void Modify(List<UIVertex> verts)
	{
		characterNumber = verts.Count / 4;
		int num = 0;
		for (int i = 0; i < verts.Count; i++)
		{
			if (i % 4 != 0)
			{
				continue;
			}
			num = i / 4;
			UIVertex value = verts[i];
			UIVertex value2 = verts[i + 1];
			UIVertex value3 = verts[i + 2];
			UIVertex value4 = verts[i + 3];
			if (usePosition)
			{
				float num2 = positionAnimationCurve.Evaluate(SeparationRate(progress, num, characterNumber, positionSeparation));
				Vector3 vector = (positionTo - positionFrom) * num2 + positionFrom;
				value.position += vector;
				value2.position += vector;
				value3.position += vector;
				value4.position += vector;
			}
			if (useScale)
			{
				if (scaleSyncXY)
				{
					float num3 = scaleAnimationCurve.Evaluate(SeparationRate(progress, num, characterNumber, scaleSeparation));
					float num4 = (scaleTo - scaleFrom) * num3 + scaleFrom;
					float x = (value2.position.x - value4.position.x) * scalePivot.x + value4.position.x;
					float y = (value2.position.y - value4.position.y) * scalePivot.y + value4.position.y;
					Vector3 vector2 = new Vector3(x, y, 0f);
					value.position = (value.position - vector2) * num4 + vector2;
					value2.position = (value2.position - vector2) * num4 + vector2;
					value3.position = (value3.position - vector2) * num4 + vector2;
					value4.position = (value4.position - vector2) * num4 + vector2;
				}
				else
				{
					float num5 = scaleAnimationCurve.Evaluate(SeparationRate(progress, num, characterNumber, scaleSeparation));
					float num6 = (scaleTo - scaleFrom) * num5 + scaleFrom;
					float x2 = (value2.position.x - value4.position.x) * scalePivot.x + value4.position.x;
					float y2 = (value2.position.y - value4.position.y) * scalePivot.y + value4.position.y;
					Vector3 vector3 = new Vector3(x2, y2, 0f);
					value.position = new Vector3(((value.position - vector3) * num6 + vector3).x, value.position.y, value.position.z);
					value2.position = new Vector3(((value2.position - vector3) * num6 + vector3).x, value2.position.y, value2.position.z);
					value3.position = new Vector3(((value3.position - vector3) * num6 + vector3).x, value3.position.y, value3.position.z);
					value4.position = new Vector3(((value4.position - vector3) * num6 + vector3).x, value4.position.y, value4.position.z);
					num5 = scaleAnimationCurveY.Evaluate(SeparationRate(progress, num, characterNumber, scaleSeparation));
					num6 = (scaleToY - scaleFromY) * num5 + scaleFromY;
					x2 = (value2.position.x - value4.position.x) * scalePivotY.x + value4.position.x;
					y2 = (value2.position.y - value4.position.y) * scalePivotY.y + value4.position.y;
					vector3 = new Vector3(x2, y2, 0f);
					value.position = new Vector3(value.position.x, ((value.position - vector3) * num6 + vector3).y, value.position.z);
					value2.position = new Vector3(value2.position.x, ((value2.position - vector3) * num6 + vector3).y, value2.position.z);
					value3.position = new Vector3(value3.position.x, ((value3.position - vector3) * num6 + vector3).y, value3.position.z);
					value4.position = new Vector3(value4.position.x, ((value4.position - vector3) * num6 + vector3).y, value4.position.z);
				}
			}
			if (useRotation)
			{
				float num7 = rotationAnimationCurve.Evaluate(SeparationRate(progress, num, characterNumber, rotationSeparation));
				float angle = (rotationTo - rotationFrom) * num7 + rotationFrom;
				float x3 = (value2.position.x - value4.position.x) * rotationPivot.x + value4.position.x;
				float y3 = (value2.position.y - value4.position.y) * rotationPivot.y + value4.position.y;
				Vector3 vector4 = new Vector3(x3, y3, 0f);
				value.position = Quaternion.AngleAxis(angle, Vector3.forward) * (value.position - vector4) + vector4;
				value2.position = Quaternion.AngleAxis(angle, Vector3.forward) * (value2.position - vector4) + vector4;
				value3.position = Quaternion.AngleAxis(angle, Vector3.forward) * (value3.position - vector4) + vector4;
				value4.position = Quaternion.AngleAxis(angle, Vector3.forward) * (value4.position - vector4) + vector4;
			}
			Color color = value.color;
			if (useColor)
			{
				float num8 = colorAnimationCurve.Evaluate(SeparationRate(progress, num, characterNumber, colorSeparation));
				color = (colorTo - colorFrom) * num8 + colorFrom;
				value.color = (value2.color = (value3.color = (value4.color = color)));
			}
			if (useAlpha)
			{
				float num9 = alphaAnimationCurve.Evaluate(SeparationRate(progress, num, characterNumber, alphaSeparation));
				float num10 = (alphaTo - alphaFrom) * num9 + alphaFrom;
				color = new Color(color.r, color.g, color.b, color.a * num10);
				value.color = (value2.color = (value3.color = (value4.color = color)));
			}
			verts[i] = value;
			verts[i + 1] = value2;
			verts[i + 2] = value3;
			verts[i + 3] = value4;
		}
	}

	private static float SeparationRate(float progress, int currentCharacterNumber, int characterNumber, float separation)
	{
		return Mathf.Clamp01((progress - (float)currentCharacterNumber * separation / (float)characterNumber) / (separation / (float)characterNumber + 1f - separation));
	}
}
