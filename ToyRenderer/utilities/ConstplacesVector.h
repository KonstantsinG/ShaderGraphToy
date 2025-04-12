#pragma once

#include "CommonHeaders.h"
#include <vector>


template <typename T>
class ConstplacesVector
{
public:
    ConstplacesVector() = default;

    ui32 add(const T& item)
    {
        ui32 id{ id::invalidId };

        if (m_availableSlots.empty())
        {
            id = static_cast<ui32>(m_data.size());
            m_data.emplace_back(item);
        }
        else
        {
            id = m_availableSlots.back();
            m_availableSlots.pop_back();
            assert(id != id::invalidId && id < m_data.size());
            m_data[id] = item;
        }

        return id;
    }

    void remove(ui32 id)
    {
        assert(id < m_data.size() && "Invalid ID");
        m_availableSlots.push_back(id);
    }

    T& operator[](ui32 id)
    {
        assert(id < m_data.size() && "Invalid ID");
        return m_data[id];
    }

    const T& operator[](ui32 id) const
    {
        assert(id < m_data.size() && "Invalid ID");
        return m_data[id];
    }

    bool isValid(ui32 id) const
    {
        return id != id::invalidId && id < m_data.size() &&
            std::find(m_availableSlots.begin(), m_availableSlots.end(), id) == m_availableSlots.end();
    }

    size_t activeSize() const
    {
        return m_data.size() - m_availableSlots.size();
    }

    size_t totalSize() const {
        return m_data.size();
    }

private:
    std::vector<T> m_data;
    std::vector<ui32> m_availableSlots;
};