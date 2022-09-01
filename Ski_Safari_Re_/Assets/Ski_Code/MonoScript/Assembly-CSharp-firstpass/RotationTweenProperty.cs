using UnityEngine;

public class RotationTweenProperty : AbstractVector3TweenProperty
{
	private bool _useLocalRotation;

	public bool useLocalRotation
	{
		get
		{
			return _useLocalRotation;
		}
	}

	public RotationTweenProperty(Vector3 endValue, bool isRelative = false, bool useLocalRotation = false)
		: base(endValue, isRelative)
	{
		_useLocalRotation = useLocalRotation;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		if (base.Equals(obj))
		{
			return _useLocalRotation == ((RotationTweenProperty)obj)._useLocalRotation;
		}
		EulerAnglesTweenProperty eulerAnglesTweenProperty = obj as EulerAnglesTweenProperty;
		if (eulerAnglesTweenProperty != null)
		{
			return _useLocalRotation == eulerAnglesTweenProperty.useLocalEulers;
		}
		return false;
	}

	public override void prepareForUse()
	{
		_target = _ownerTween.target as Transform;
		_endValue = _originalEndValue;
		if (_ownerTween.isFrom)
		{
			_startValue = _endValue;
			if (_useLocalRotation)
			{
				_endValue = _target.localRotation.eulerAngles;
			}
			else
			{
				_endValue = _target.rotation.eulerAngles;
			}
		}
		else if (_useLocalRotation)
		{
			_startValue = _target.localRotation.eulerAngles;
		}
		else
		{
			_startValue = _target.rotation.eulerAngles;
		}
		if (_isRelative && !_ownerTween.isFrom)
		{
			_diffValue = _startValue + _endValue;
		}
		else
		{
			_diffValue = new Vector3(Mathf.DeltaAngle(_startValue.x, _endValue.x), Mathf.DeltaAngle(_startValue.y, _endValue.y), Mathf.DeltaAngle(_startValue.z, _endValue.z));
		}
	}

	public override void tick(float totalElapsedTime)
	{
		float value = _easeFunction(totalElapsedTime, 0f, 1f, _ownerTween.duration);
		Vector3 euler = GoTweenUtils.unclampedVector3Lerp(_startValue, _diffValue, value);
		if (_useLocalRotation)
		{
			_target.localRotation = Quaternion.Euler(euler);
		}
		else
		{
			_target.rotation = Quaternion.Euler(euler);
		}
	}
}
