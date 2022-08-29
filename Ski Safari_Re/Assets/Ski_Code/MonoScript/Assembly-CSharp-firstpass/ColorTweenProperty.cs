using System;
using UnityEngine;

public class ColorTweenProperty : AbstractColorTweenProperty, IGenericProperty
{
	private Action<Color> _setter;

	public string propertyName { get; private set; }

	public ColorTweenProperty(string propertyName, Color endValue, bool isRelative = false)
		: base(endValue, isRelative)
	{
		this.propertyName = propertyName;
	}

	public override bool validateTarget(object target)
	{
		_setter = GoTweenUtils.setterForProperty<Action<Color>>(target, propertyName);
		return _setter != null;
	}

	public override void prepareForUse()
	{
		Func<Color> func = GoTweenUtils.getterForProperty<Func<Color>>(_ownerTween.target, propertyName);
		_endValue = _originalEndValue;
		if (_ownerTween.isFrom)
		{
			_startValue = _endValue;
			_endValue = func();
		}
		else
		{
			_startValue = func();
		}
		base.prepareForUse();
	}

	public override void tick(float totalElapsedTime)
	{
		float value = _easeFunction(totalElapsedTime, 0f, 1f, _ownerTween.duration);
		Color obj = GoTweenUtils.unclampedColorLerp(_startValue, _diffValue, value);
		_setter(obj);
	}
}
