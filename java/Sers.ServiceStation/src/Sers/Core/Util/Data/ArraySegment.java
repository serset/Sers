package Sers.Core.Util.Data;

public class ArraySegment  {
    public ArraySegment(){
        array=null;
        offset=0;
        count=0;
    }
    public ArraySegment(byte[] array, int offset, int count){
        this.array=array;
        this.offset=offset;
        this.count=count;
    }
    public ArraySegment(byte[] array){
    	this.array=array;
    	this.count= (array==null?0:array.length);
    }

    public void CopyFrom(ArraySegment arr){
        if(arr!=null){
            array=arr.array;
            offset=arr.offset;
            count=arr.count;
        }
    }


    public byte Get(int index){
        return  array[index+offset];
    }

    public ArraySegment Slice(int Offset){
        return new ArraySegment(array,this.offset+Offset,this.count-Offset);
    }

    public ArraySegment Slice(int Offset,int Count){
        return new ArraySegment(array,this.offset+Offset,Count);
    }

    
    public byte[]  ToBytes(){
    	if(array==null) {
    		return null;
    	}
    	if(count<=0) {
    		return new byte[0];
    	}
    	 byte[] data=new byte[count];
    	 System.arraycopy(array, offset, data, 0, count);
    	 return data;    	
    }
    
    

    //region long <--> byte[]
    /**
     *
     * @param value
     * @param bytes
     * @param offset
     */
    public static void int64ToBytes(long value, byte[] bytes,int offset) {
        for (int i = 0; i <8; i++) {
            bytes[i+offset] = (byte) (value  & 0xff);
            value>>>=8;
        }
    }

    /**
     *
     * @param bytes
     * @param offset
     * @return
     */
    public static long bytesToInt64(byte[] bytes, int offset){
        long value=0;

        for (int i = 7; i >=0; i--) {
            value<<=8;
            value|= (bytes[offset+i]&0xff);
        }
        return value;
    }


    public long ReadLong(int offset){
        return bytesToInt64(array,this.offset+offset);
    }

    public  ArraySegment WriteLong(long value,int offset){
        int64ToBytes(value,array,this.offset+offset);
        return this;
    }
    //endregion


    //region int32 <--> byte[]
    /**
     *
     * @param value
     * @param bytes
     * @param offset
     */
    public static void int32ToBytes(int value, byte[] bytes,int offset) {
        for (int i = 0; i <4; i++) {
            bytes[i+offset] = (byte) (value  & 0xff);
            value>>>=8;
        }
    }

    /**
     *
     * @param bytes
     * @param offset
     * @return
     */
    public static int bytesToInt32(byte[] bytes, int offset){
        int value=0;

        for (int i = 3; i >=0; i--) {
            value<<=8;
            value|= (bytes[offset+i]&0xff);
        }
        return value;
    }


    public int ReadInt32(int offset){
        return bytesToInt32(array,this.offset+offset);
    }

    public  ArraySegment WriteInt32(int value,int offset){
        int32ToBytes(value,array,this.offset+offset);
        return this;
    }
    //endregion

    //region static Int32ToArraySegmentByte
    public static ArraySegment Int32ToArraySegmentByte(int value) {
    	ArraySegment arr=new ArraySegment(new byte[4]);
    	
    	arr.WriteInt32(value, 0);
    	return arr;
    }
    
    //endregion


    //
    // 摘要:
    //     Gets the original array containing the range of elements that the array segment
    //     delimits.
    //
    // 返回结果:
    //     The original array that was passed to the constructor, and that contains the
    //     range delimited by the System.ArraySegment`1.
    public byte[] array=null ;
    //
    // 摘要:
    //     Gets the number of elements in the range delimited by the array segment.
    //
    // 返回结果:
    //     The number of elements in the range delimited by the System.ArraySegment`1.
    public int count=0;
    //
    // 摘要:
    //     Gets the position of the first element in the range delimited by the array segment,
    //     relative to the start of the original array.
    //
    // 返回结果:
    //     The position of the first element in the range delimited by the System.ArraySegment`1,
    //     relative to the start of the original array.
    public int offset=0;
}
