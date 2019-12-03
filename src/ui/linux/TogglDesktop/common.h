// Copyright 2019 Toggl Desktop developers.

#ifndef SRC_COMMON_H_
#define SRC_COMMON_H_

#include <QMutex>

#ifndef TOGGL_SAFE_PROPERTIES

#define PROPERTY(type, name, ...) \
private: Q_PROPERTY(type name READ name##Get WRITE name##Set NOTIFY name##Changed) \
public: type name { __VA_ARGS__ }; \
        type name##Get() const { \
            auto value = name; \
            return value; \
        } \
        Q_SLOT Q_INVOKABLE void name##Set(const type &val) { \
            if (name != val) { \
                name = val; \
                emit name##Changed(); \
            } \
        } \
        Q_SIGNAL void name##Changed();

#else // TOGGL_SAFE_PROPERTIES

#define PROPERTY(type, name, ...) \
private: Q_PROPERTY(type name READ name##Get WRITE name##Set NOTIFY name##Changed) \
public: type name { __VA_ARGS__ }; \
        type name##Get() const { \
            propertyMutex_.lock(); \
            auto value = name; \
            propertyMutex_.unlock(); \
            return value; \
        } \
        Q_SLOT Q_INVOKABLE void name##Set(const type &val) { \
            propertyMutex_.lock(); \
            if (name != val) { \
                name = val; \
                propertyMutex_.unlock(); \
                emit name##Changed(); \
            } \
            else \
                propertyMutex_.unlock(); \
        } \
        Q_SIGNAL void name##Changed();

#endif // TOGGL_SAFE_PROPERTIES

#endif // SRC_COMMON_H_