package Sers.Core.Util.Data;

public class ArraySegment  {
    public ArraySegment(){
        Array=null;
        Offset=0;
        Count=0;
    }
    public ArraySegment(byte[] array, int offset, int count){
        Array=array;
        Offset=offset;
        Count=count;
    }
    public ArraySegment(byte[] array){
        Array=array;
        Count=array.length;
    }

    public void CopyFrom(ArraySegment arr){
        if(arr!=null){
            Array=arr.Array;
            Offset=arr.Offset;
            Count=arr.Count;
        }
    }


    public byte Get(int index){
        return  Array[index+Offset];
    }

    public ArraySegment Slice(int Offset){
        return new ArraySegment(Array,this.Offset+Offset,this.Count-Offset);
    }

    public ArraySegment Slice(int Offset,int Count){
        return new ArraySegment(Array,this.Offset+Offset,Count);
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
        return bytesToInt64(Array,this.Offset+offset);
    }

    public  ArraySegment WriteLong(long value,int offset){
        int64ToBytes(value,Array,this.Offset+offset);
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
        return bytesToInt32(Array,this.Offset+offset);
    }

    public  ArraySegment WriteInt32(int value,int offset){
        int32ToBytes(value,Array,this.Offset+offset);
        return this;
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
    public byte[] Array=null ;
    //
    // 摘要:
    //     Gets the number of elements in the range delimited by the array segment.
    //
    // 返回结果:
    //     The number of elements in the range delimited by the System.ArraySegment`1.
    public int Count=0;
    //
    // 摘要:
    //     Gets the position of the first element in the range delimited by the array segment,
    //     relative to the start of the original array.
    //
    // 返回结果:
    //     The position of the first element in the range delimited by the System.ArraySegment`1,
    //     relative to the start of the original array.
    public int Offset=0;
}
