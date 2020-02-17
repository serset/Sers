/*
 * ByteData.h
 *
 *  Created on: 2019年3月20日
 *      Author: lith
 */

#ifndef SERS_CORE_DATA_BYTEDATA_HPP_
#define SERS_CORE_DATA_BYTEDATA_HPP_


#include <stdio.h>
#include <memory>
#include <string.h>

namespace std{

	template <typename T>
	shared_ptr<T> make_shared_array(size_t size)
	{
		//default_delete是STL中的默认删除器
		return shared_ptr<T>(new T[size], default_delete<T[]>());
	}

	template <typename T>
	shared_ptr<T> make_shared_array(T * ptr)
	{
		//default_delete是STL中的默认删除器
		return shared_ptr<T>(ptr, default_delete<T[]>());
	}
}






namespace Sers {


using namespace std;

class ByteData;


class ArraySegment
{
private:
		shared_ptr<char> ptr;
public:

	void Malloc(int len)
	{
		this->ptr=make_shared_array<char>(len);
		this->len=len;
		offset=0;
	}

	void  CopyFrom(const ArraySegment & v)
	{
		Malloc(v.len);
		memcpy(ptr.get(),v.GetData(),len);
	}

	void  CopyFrom(const char * str,int len)
	{
		Malloc(len);
		memcpy( ptr.get(),str,len);
	}

	void  CopyFrom(const char * str)
	{
		CopyFrom(str,strlen(str));
	}


	// indicate no data if  less than 0
	int len;
	int offset;


	ArraySegment& operator =(const ArraySegment & v)
	{
		this->ptr=v.ptr;
		this->len=v.len;
		this->offset=v.offset;
		return *this;
	}


	ArraySegment(shared_ptr<char> ptr,int len,int offset=0)
	{
    	SetData(ptr,len,offset);
	}

	ArraySegment( )
	{
		len=0;
		offset=0;
	}

//	ArraySegment(char * ptr=nullptr,int len=-1,int offset=0)
//	{
//		SetData(ptr,len,offset);
//	}

	ArraySegment(const ArraySegment & v)
	{
    	ptr=v.ptr;
    	len=v.len;
    	offset=v.offset;
	}


	bool IsEmpty()
	{
		return  len<0;
	}

	void Empty()
	{
		if(IsEmpty()) return;

		ptr.reset();
		len=-1;
		offset=0;

	}




//	void SetData(char * ptr=nullptr,int len=-1,int offset=0)
//	{
//		if(ptr!=nullptr)
//		    		this->ptr=make_shared_array(ptr);
//		this->len=len;
//		this->offset=offset;
//	}

	void SetData(const shared_ptr<char> & ptr,int len,int offset=0)
	{
		this->ptr=ptr;
		this->len=len;
		this->offset=offset;
	}

//	char * GetOriData() const
//	{
//		return this->ptr.get();
//	}

	 char *  GetData() const
	{
		return this->ptr.get()+offset;
	}

	ArraySegment Slice(int offset,int len)const
	{
		return ArraySegment(this->ptr,len,offset);
	}

	ArraySegment Slice(int offset) const
	{
		return ArraySegment(this->ptr,this->len-offset,offset);
	}

};




class ByteData_Item
{

public:

	ArraySegment data;
	ByteData_Item * next;
};



class ByteData
{
private:

	int len;


public:
	ByteData_Item * head;


	ByteData()
	{
		len=0;
		head=nullptr;
	}
	~ByteData()
	{
		Empty();
	}


	void Empty()
	{
		if(IsEmpty()) return;

		ByteData_Item*next, *cur=head;
		while(cur)
		{
			next=cur->next;
			delete cur;
			cur=next;
		}
		len=0;
		head=nullptr;
	}

	// insert data to head
	void InsertData(const ArraySegment& data)
	{
		ByteData_Item * cur =new ByteData_Item();

		cur->data=data;

		cur->next=head;

		head=cur;

		this->len+=data.len;
	}


	ArraySegment ToArraySegment()
	{
		ArraySegment data;
		data.Malloc(len);

		char * ptr=data.GetData();
		auto cur= head;
		while(cur)
		{
			memcpy(ptr,cur->data.GetData(),cur->data.len);
			ptr+=cur->data.len;
			cur=cur->next;
		}
		return data;
	}

	bool IsEmpty()
	{
		return !head;
	}
	int Length()
	{
		return len;
	}

};




} /* namespace Sers */

#endif /* SERS_CORE_DATA_BYTEDATA_HPP_ */
