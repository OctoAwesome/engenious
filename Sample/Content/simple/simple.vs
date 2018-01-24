#if __VERSION__ >= 130
uniform mat4 WorldViewProj;

in uint inputData;
in uint inputData2;

out vec3 psNormal;
out vec2 psTexcoord;
flat out uint psTexIndex;

void main()
{
	const vec2[] uvLookup = vec2[4](vec2(05.0,0.0),vec2(0.0,1.0),vec2(1.0,0.0),vec2(1.0,1.0));
	
	const vec3[] normalLookup = vec3[6](vec3(1.0,0.0,0.0),vec3(-1.0,0.0,0.0),vec3(0.0,1.0,0.0),vec3(0.0,-1.0,0.0),vec3(0.0,0.0,1.0),vec3(0.0,0.0,-1.0));
	vec4 position = vec4((inputData & 0xFFu),((inputData >> 8) & 0xFFu),((inputData >> 16) & 0xFFu),1.0);
	psNormal = normalLookup[(inputData2 >> 24) & 0xFu];
	psTexIndex = (inputData>>24);
	psTexcoord = uvLookup[(inputData2 >> 28) & 0xFu];
	

	gl_Position = WorldViewProj*position;


}
#else
const int BIT_COUNT = 8;

int modi(int x, int y) {
    return x - y * (x / y);
}

int or(int a, int b) {
    int result = 0;
    int n = 1;

    for(int i = 0; i < BIT_COUNT; i++) {
        if ((modi(a, 2) == 1) || (modi(b, 2) == 1)) {
            result += n;
        }
        a = a / 2;
        b = b / 2;
        n = n * 2;
        if(!(a > 0 || b > 0)) {
            break;
        }
    }
    return result;
}

int and(int a, int b) {
    int result = 0;
    int n = 1;

    for(int i = 0; i < BIT_COUNT; i++) {
        if ((modi(a, 2) == 1) && (modi(b, 2) == 1)) {
            result += n;
        }

        a = a / 2;
        b = b / 2;
        n = n * 2;

        if(!(a > 0 && b > 0)) {
            break;
        }
    }
    return result;
}

int not(int a) {
    int result = 0;
    int n = 1;
    
    for(int i = 0; i < BIT_COUNT; i++) {
        if (modi(a, 2) == 0) {
            result += n;    
        }
        a = a / 2;
        n = n * 2;
    }
    return result;
}

uniform mat4 WorldViewProj;

in uint inputData;
in uint inputData2;

out vec3 psNormal;
out vec2 psTexcoord;
flat out uint psTexIndex;

void main()
{
	const vec2[] uvLookup = vec2[4](vec2(05.0,0.0),vec2(0.0,1.0),vec2(1.0,0.0),vec2(1.0,1.0));
	
	const vec3[] normalLookup = vec3[6](vec3(1.0,0.0,0.0),vec3(-1.0,0.0,0.0),vec3(0.0,1.0,0.0),vec3(0.0,-1.0,0.0),vec3(0.0,0.0,1.0),vec3(0.0,0.0,-1.0));
	vec4 position = vec4(and(inputData, 0xFF),and((inputData / 0x100), 0xFF),and((inputData / 0x10000), 0xFF),1.0);
	psNormal = normalLookup[and((inputData2 / 0x1000000), 0xF)];
	psTexIndex = (inputData>>24);
	psTexcoord = uvLookup[and((inputData2 / 0x10000000), 0xF)];
	

	gl_Position = WorldViewProj*position;


}

#endif
