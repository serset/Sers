#include "../../../../Sers/Core/Data/ByteData.hpp"

using namespace Sers::Core::Data;


void ArraySegment_Demo()
{
	char * ptr=new char[100];

	ptr[1]=121;

	ArraySegment a1;
	a1.SetData(ptr,100);

	char c1=a1.GetData()[1];


	ArraySegment a2=a1.Slice(1);

	a1.Empty();

	char c2=a2.GetData()[0];

	a2.Empty();

	char * ptr2=new char[100];

	//c1==c2,1
	printf("c1==c2,%d",c1==c2);
	//ptr==ptr2,1
	printf("ptr==ptr2,%d",ptr==ptr2);

}
