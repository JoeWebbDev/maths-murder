using Godot;
using System;

public partial class HealthBar : Node2D
{
    private static class ShaderParameters
    {
        public static string FillAmount = "fill_amount";
        public static string UnderlyingColorFillAmount = "underlying_color_fill_amount";
        public static string FillTexture = "sample_fill_texture";
    }
    
    [Export] private float _underlyingColorDelayTime;
    [Export] private float _underlyingColorSpeed;

    [ExportGroup("Internal scene properties")]
    [Export] private Sprite2D _border;
    [Export] private Sprite2D _fill;

    private float _maxValue;
    private float _value;
    private float _underlyingValue;
    private float _currentUnderlyingValueDelay;
    private bool _isUpdatingUnderlyingValue;
    private bool _useUnderlyingValue = true;
    private bool _useBorderFrame = true;
    private ShaderMaterial _fillShader;
    
    public float CurrentValueAsDecimal => _value > 0 ? _value / _maxValue : 0;

    public override void _Ready()
    {
        _fillShader = (_fill.Material as ShaderMaterial);
    }

    public override void _Process(double delta)
    {
        if (_useUnderlyingValue) UpdateShaderUnderlyingColor((float)delta);
    }

    public void Initialize(float value, float maxValue, bool useUnderlyingValue = true, bool useBorderFrame = true)
    {
        _maxValue = maxValue;
        _value = value;
        _useUnderlyingValue = useUnderlyingValue;
        _underlyingValue = useUnderlyingValue ? value : 0;
        _useBorderFrame = useBorderFrame;
        UpdateShader();
    }

    public void UpdateValue(float to)
    {
        _value = to;
        UpdateShader();
        TryUpdateBorderFrame();
    }

    public void UpdateFillTexture(Texture2D tex)
    {
        _fillShader.SetShaderParameter(ShaderParameters.FillTexture, tex);
    }

    private void UpdateShader()
    {
        _fillShader.SetShaderParameter(ShaderParameters.FillAmount, CurrentValueAsDecimal);
        if (!_isUpdatingUnderlyingValue) _currentUnderlyingValueDelay = _underlyingColorDelayTime;
    }

    private void TryUpdateBorderFrame()
    {
        if (!_useBorderFrame) return;
        var targetFrame = Mathf.Abs(Mathf.CeilToInt(CurrentValueAsDecimal * 4) - 4);
        if (_border.Frame != targetFrame) _border.Frame = targetFrame;
    }

    private void UpdateShaderUnderlyingColor(float delta)
    {
        if (_underlyingValue <= _value)
        {
            _isUpdatingUnderlyingValue = false;
            return;
        }
        if (_currentUnderlyingValueDelay > 0)
        {
            _currentUnderlyingValueDelay -= delta;
            return;
        }
        _isUpdatingUnderlyingValue = true;
        _underlyingValue -= delta * _underlyingColorSpeed;
        var underlyingValueAsFraction = _underlyingValue > 0 ? _underlyingValue / _maxValue : 0;
        _fillShader.SetShaderParameter(ShaderParameters.UnderlyingColorFillAmount, underlyingValueAsFraction);
    }
}
