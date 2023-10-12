shader_type canvas_item;

uniform sampler2D sample_fill_texture;
uniform vec4 underlying_color : source_color = vec4(vec3(1.0), 1.0);
uniform float fill_amount : hint_range(0.0, 1.0) = 0.5;
uniform float underlying_color_fill_amount : hint_range(0.0, 1.0) = 0.5;

void fragment() {
	vec4 tex = texture(TEXTURE, UV);
	vec2 sample_tex_pos = vec2(fill_amount, fill_amount);
	vec2 underlying_color_pos = vec2(underlying_color_fill_amount, underlying_color_fill_amount);
	vec4 sample_tex = texture(sample_fill_texture, sample_tex_pos);
	// Filter out transparent pixels
	if (tex.a > 0.1) {
		COLOR = sample_tex;
		if (UV.x > sample_tex_pos.x) COLOR.a = 0.0;
		if (UV.x > sample_tex_pos.x && UV.x < underlying_color_pos.x) COLOR = underlying_color;
	}
}