//to do
//lookup table //LUT
typedef struct pixel{
    unsigned int x;
    unsigned int y;
    unsigned char value;
} pixel;

int main(){
    pixel p;
    p.x = 0.0f;
    p.y = 0.0f;
    p.value = 0;
    printf("%d\n%d\n%d\n",p.x,p.y,p.value);
    return 0;
}