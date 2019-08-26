/*
 * ConcurrentMap.h
 *
 *  Created on: 2019年3月18日
 *      Author: root
 */

#ifndef SERS_CORE_UTIL_CONCURRENT_CONCURRENTMAP_H_
#define SERS_CORE_UTIL_CONCURRENT_CONCURRENTMAP_H_

#include <map>



#if defined  _WIN32                                                         //Windows
#include <Windows.h>
#define MyMap_CLock_Mutex_t                 HANDLE
#define MyMap_CLock_Mutex_Init(_mutex)      (_mutex = CreateSemaphore(NULL,1,1,NULL))
#define MyMap_CLock_Mutex_Lock(_mutex)      (WaitForSingleObject(_mutex, INFINITE))
#define MyMap_CLock_Mutex_UnLock(_mutex)    (ReleaseSemaphore(_mutex,1,NULL))
#define MyMap_CLock_Mutex_Destroy(_mutex)   (CloseHandle(_mutex))
#define MyMap_Declar_Typename               typename
#define MyMap_Type_Typename

#elif defined __linux                                                       //Linux
#include <pthread.h>
#define MyMap_CLock_Mutex_t                 pthread_mutex_t
#define MyMap_CLock_Mutex_Init(_mutex)      (pthread_mutex_init(&_mutex, NULL))
#define MyMap_CLock_Mutex_Lock(_mutex)      (pthread_mutex_lock(&_mutex))
#define MyMap_CLock_Mutex_UnLock(_mutex)    (pthread_mutex_unlock(&_mutex))
#define MyMap_CLock_Mutex_Destroy(_mutex)   (pthread_mutex_destroy(&_mutex))
#define MyMap_Declar_Typename
#define MyMap_Type_Typename                 typename
#endif

namespace Sers {
namespace Core {
namespace Util {
namespace Concurrent {

    //my iterator
    template<class K, class V>
    class WDItr
    {
    public:
        WDItr();
        WDItr(const std::map<K, V>& t_map);
        WDItr(const WDItr& val);
        ~WDItr();


        //************************************
        // Method:    operator
        // FullName:  operator
        // Access:    public
        // Returns:
        // Qualifier: 迭代器没有实现自减运算符重载
        // Parameter:
        //************************************
        WDItr<K, V>& operator=(const WDItr& val);
        WDItr<K, V>& operator++(int);
        bool operator==(const WDItr& val);
        bool operator!=(const WDItr& val);
        WDItr<K, V>& operator++();

    public:
        bool _isEnd;

        K first;
        V second;

    private:
        std::map<K, V> _tempMap;

        WDItr<K, V>* _oldSelf;
    };

    template<class K, class V>
    bool Concurrent::WDItr<K, V>::operator!=( const WDItr& val )
    {
        return !(*this == val);
    }
    template<class K, class V>
    WDItr<K, V>& Concurrent::WDItr<K, V>::operator++()
    {
        MyMap_Type_Typename std::map<K, V>::iterator _mitr = _tempMap.find(this->first);

        if(_mitr == _tempMap.end() || ++_mitr == _tempMap.end())
        {
            this->_isEnd = true;

            return *this;
        }

        this->_isEnd = false;
        this->first = _mitr->first;
        this->second = _mitr->second;

        return *this;
    }

    template<class K, class V>
    bool Concurrent::WDItr<K, V>::operator==( const WDItr& val )
    {
        if(this->_isEnd == true && val._isEnd == true)
        {
            return true;
        }

        if(this->_isEnd == val._isEnd && this->first == val.first)
        {
            return true;
        }

        return false;
    }
    template<class K, class V>
    WDItr<K, V>& Concurrent::WDItr<K, V>::operator++( int )
    {
        if(this->_oldSelf)
        {
            delete this->_oldSelf;
            this->_oldSelf = NULL;
        }

        this->_oldSelf = new WDItr<K, V>(*this);

        ++(*this);

        return *(this->_oldSelf);
    }
    template<class K, class V>
    WDItr<K, V>& Concurrent::WDItr<K, V>::operator=( const WDItr& val )
    {
        if(this == &val)
        {
            return *this;
        }

        this->first = val.first;
        this->second = val.second;
        this->_isEnd = val._isEnd;
        this->_tempMap = val._tempMap;
        this->_oldSelf = NULL;

        if(val._oldSelf)
        {
            this->_oldSelf = new WDItr<K, V>(*val._oldSelf);
        }

        return *this;
    }
    template<class K, class V>
    Concurrent::WDItr<K, V>::WDItr( const WDItr& val )
    {
        this->first = val.first;
        this->second = val.second;
        this->_isEnd = val._isEnd;
        this->_tempMap = val._tempMap;
        this->_oldSelf = NULL;

        if(val._oldSelf)
        {
            this->_oldSelf = new WDItr<K, V>(*val._oldSelf);
        }
    }
    template<class K, class V>
    Concurrent::WDItr<K, V>::WDItr( const std::map<K, V>& t_map )
    {
        this->_isEnd = true;
       this->_tempMap = t_map;
        this->_oldSelf = NULL;
    }
    template<class K, class V>
    Concurrent::WDItr<K, V>::~WDItr()
    {
        if(this->_oldSelf)
        {
            delete this->_oldSelf;
            this->_oldSelf = NULL;
        }
    }
    template<class K, class V>
    Concurrent::WDItr<K, V>::WDItr()
    {
        this->_isEnd = true;
        this->_oldSelf = NULL;
    }

    //lock
    class CLock
    {
    public:
        CLock(){MyMap_CLock_Mutex_Init(_mutex);}
        ~CLock(){MyMap_CLock_Mutex_Destroy(_mutex);}

        void Lock() {MyMap_CLock_Mutex_Lock(_mutex);}
        void UnLock() {MyMap_CLock_Mutex_UnLock(_mutex);}

    private:
        MyMap_CLock_Mutex_t _mutex;
    };

    //ConcurrentMap
    template<class K, class V>
    class ConcurrentMap
    {
    public:
        ConcurrentMap(void);

        ~ConcurrentMap(void);

        typedef MyMap_Declar_Typename WDItr<K, V> iterator;
        typedef unsigned int size_type;

        V& operator[](const K& t_key);


    public:
        iterator begin();
        iterator end();

        void insert(const K t_key, const V t_val);
        iterator find(const K t_key);
        void erase(const K t_key);
        void erase(const iterator _val);
        bool update(const K t_key, const V t_val);
        bool update(const iterator _val);

        size_type size();

    private:
        std::map<K, V> _Concurrent;

        CLock _myLock;
    };

    template<class K, class V>
    V& Concurrent::ConcurrentMap<K, V>::operator[]( const K& t_key )
    {
        this->_myLock.Lock();

        MyMap_Type_Typename std::map<K, V>::iterator _mitr = this->_Concurrent.find(t_key);
        if(_mitr == this->_Concurrent.end())
        {
            this->_Concurrent.insert(MyMap_Type_Typename std::map<K, V>::value_type(t_key, V()));
             _mitr = this->_Concurrent.find(t_key);
        }

        this->_myLock.UnLock();

        return _mitr->second;
    }

    template<class K, class V>
    typename Concurrent::ConcurrentMap<K, V>::size_type Concurrent::ConcurrentMap<K, V>::size()
    {
        this->_myLock.Lock();

        Concurrent::ConcurrentMap<K, V>::size_type m_size = this->_Concurrent.size();

        this->_myLock.UnLock();

        return m_size;
    }
    //************************************
    // Method:    update
    // FullName:  Concurrent::map<K, V>::update
    // Access:    public
    // Returns:   bool
    // Qualifier: 通过iterator更新map中具有的first key的value值，若无该key，则返回false
    // Parameter: const iterator _val
    //************************************
    template<class K, class V>
    bool Concurrent::ConcurrentMap<K, V>::update( const iterator _val )
    {
        return update(_val.first, _val.second);
    }
    //************************************
    // Method:    update
    // FullName:  Concurrent::map<K, V>::update
    // Access:    public
    // Returns:   bool
    // Qualifier: 通过key和value更新map中具有的key的value值，若无该key，则返回false
    // Parameter: const K t_key
    // Parameter: const V t_val
    //************************************
    template<class K, class V>
    bool Concurrent::ConcurrentMap<K, V>::update( const K t_key, const V t_val )
    {
        this->_myLock.Lock();
        bool ret = false;
        MyMap_Type_Typename std::map<K, V>::iterator _mitr = this->_Concurrent.find(t_key);
        if(_mitr != this->_Concurrent.end())
        {
            _mitr->second = t_val;
            ret = true;
        }

        this->_myLock.UnLock();

        return ret;
    }
    template<class K, class V>
    void Concurrent::ConcurrentMap<K, V>::erase( const  iterator _val )
    {
        this->_myLock.Lock();

        this->_Concurrent.erase(_val.first);

        this->_myLock.UnLock();
    }
    template<class K, class V>
    void Concurrent::ConcurrentMap<K, V>::erase( const K t_key )
    {
        this->_myLock.Lock();

        this->_Concurrent.erase(t_key);

        this->_myLock.UnLock();
    }
    //************************************
    // Method:    find
    // FullName:  Concurrent::map<K, V>::find
    // Access:    public
    // Returns:   ::iterator
    // Qualifier: 通过key进行查找，若没有查找到，则返回end
    // Parameter: const K t_key
    //************************************
    template<class K, class V>
    typename Concurrent::ConcurrentMap<K, V>::iterator Concurrent::ConcurrentMap<K, V>::find( const K t_key )
    {
        this->_myLock.Lock();

        Concurrent::ConcurrentMap<K, V>::iterator _tmpItr(this->_Concurrent);

        MyMap_Type_Typename std::map<K, V>::iterator _mitr = this->_Concurrent.find(t_key);
        if(_mitr != this->_Concurrent.end())
        {
            _tmpItr._isEnd = false;
            _tmpItr.first = _mitr->first;
            _tmpItr.second = _mitr->second;
        }

        this->_myLock.UnLock();

        return _tmpItr;
    }
    //************************************
    // Method:    insert
    // FullName:  Concurrent::map<K, V>::insert
    // Access:    public
    // Returns:
    // Qualifier: 插入key，value
    // Parameter: const K t_key
    // Parameter: const V t_val
    //************************************
    template<class K, class V>
    void Concurrent::ConcurrentMap<K, V>::insert( const K t_key, const V t_val )
    {
        this->_myLock.Lock();

        this->_Concurrent.insert(MyMap_Type_Typename std::map<K, V>::value_type(t_key, t_val));

        this->_myLock.UnLock();

        return ;
    }
    template<class K, class V>
    typename Concurrent::ConcurrentMap<K, V>::iterator Concurrent::ConcurrentMap<K, V>::end()
    {
        Concurrent::ConcurrentMap<K, V>::iterator _tmpItr;

        return _tmpItr;
    }
    template<class K, class V>
    typename Concurrent::ConcurrentMap<K, V>::iterator Concurrent::ConcurrentMap<K, V>::begin()
    {
        this->_myLock.Lock();

        Concurrent::ConcurrentMap<K, V>::iterator _tmpItr(this->_Concurrent);

        MyMap_Type_Typename std::map<K, V>::iterator _mitr = this->_Concurrent.begin();
        if(_mitr != this->_Concurrent.end())
        {
            _tmpItr._isEnd = false;
            _tmpItr.first = _mitr->first;
            _tmpItr.second = _mitr->second;
        }

        this->_myLock.UnLock();

        return _tmpItr;
    }
    template<class K, class V>
    Concurrent::ConcurrentMap<K, V>::~ConcurrentMap( void )
    {
    }
    template<class K, class V>
    Concurrent::ConcurrentMap<K, V>::ConcurrentMap( void )
    {
    }

} /* namespace Concurrent */
} /* namespace Util */
} /* namespace Core */
} /* namespace Sers */

#endif /* SERS_CORE_UTIL_CONCURRENT_CONCURRENTMAP_H_ */
