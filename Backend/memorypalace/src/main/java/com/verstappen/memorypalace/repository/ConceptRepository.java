package com.verstappen.memorypalace.repository;

import java.util.List;

import org.springframework.data.jpa.repository.JpaRepository;

import com.verstappen.memorypalace.model.Concept;

public interface ConceptRepository extends JpaRepository<Concept, Long> {
    List<Concept> findByTitleContainingIgnoreCase(String keyword);

    List<Concept> findAllByOrderByIdAsc();
}
