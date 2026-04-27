shader_type canvas_item;
uniform vec3 tint_color : source_color = vec3(1.0, 0.0, 0.0);
uniform vec3 blip_color : source_color = vec3(1.0, 1.0, 1.0);
uniform float blip_strength : hint_range(0, 1) = 0.0;
uniform float intensity : hint_range(0, 1) = 1.0;


uniform float amplitude: hint_range(0.0, 1.0);
uniform float speed: hint_range(0.0, 2.0);

void fragment() {
    
    vec4 front = texture(TEXTURE, UV);
    vec3 final_color = mix(tint_color, blip_color, blip_strength);
    
    COLOR = mix(front, vec4(final_color, front.a), intensity);
    
}

void vertex() {
	// Called for every vertex the material is visible on.
	float time = TIME * (30. * speed);
	float wobbling = sin(time) * 40. * amplitude;
	float direction = sin(time / 2.);
	VERTEX += vec2(direction * wobbling,wobbling);
	//VERTEX += direction;
}