#version 330 core
out vec4 FragColor;

uniform float u_Time;
uniform vec2 u_Resolution;

const float PI = 3.141592654;

vec3 hash32(vec2 p)
{
	vec3 p3 = fract(vec3(p.xyx) * vec3(.1031, .1030, .0973));
    p3 += dot(p3, p3.yxz+19.19);
    return fract((p3.xxy+p3.yzz)*p3.zyx);
}

vec4 disco(vec2 uv) {
    float v = abs(cos(uv.x * PI * 2.) + cos(uv.y *PI * 2.)) * .5;
    uv.x -= .5;
    vec3 cid2 = hash32(vec2(floor(uv.x - uv.y), floor(uv.x + uv.y))); // generate a color
    return vec4(cid2, v);
}

void main()
{
    vec2 R = u_Resolution.xy;
    vec2 uv = gl_FragCoord.xy / R.xy;
    uv.x *= R.x / R.y; // aspect correct

    float t = u_Time * .6; //t = 0.;
    uv *= 8.;
    uv -= vec2(t*.5, -t*.3);
    
    FragColor = vec4(1);
    for(float i = 1.; i <= 4.; ++i) {
        uv /= i*.9;
        vec4 d = disco(uv);
        float curv = pow(d.a, .44-((1./i)*.3));
        curv = pow(curv, .8+(d.b * 2.));
        FragColor *= clamp(d * curv,.35, 1.);
        uv += t*(i+.3);
    }
    
    // post
    FragColor = clamp(FragColor,.0,1.);
    vec2 N = (gl_FragCoord.xy / R.xy )- .5;
    FragColor = 1.-pow(1.-FragColor, vec4(30.));// curve
    FragColor.rgb += hash32(gl_FragCoord.xy + u_Time).r*.07;//noise
    FragColor *= 1.0-dot(N,N*1.7);// vingette
    FragColor.a = 1.;
}