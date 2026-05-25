package com.verstappen.memorypalace.controller;

import java.io.IOException;
import java.util.List;

import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PatchMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import com.verstappen.memorypalace.dto.ConceptDTO;
import com.verstappen.memorypalace.dto.ReviewDTO;
import com.verstappen.memorypalace.model.Concept;
import com.verstappen.memorypalace.service.ConceptService;

import jakarta.validation.Valid;

@RestController
@RequestMapping("/concepts")
public class ConceptController {

    private final ConceptService service;

    public ConceptController(ConceptService service) {
        this.service = service;
    }

    @PostMapping
    public Concept add(@Valid @RequestBody ConceptDTO dto) throws IOException {
        return service.save(dto);
    }

    @GetMapping
    public Page<Concept> getAll(Pageable pageable) {
        return service.getPaginated(pageable);
    }

    /**
     * Returns the COMPLETE list of concepts ordered by id ascending (no pagination).
     *
     * Frontend function: getAllSorted()  →  GET /concepts/all-sorted
     *
     * This is the endpoint the frontend calls to load all concepts for the
     * Memory Palace review UI.  The ascending-order list can be extracted
     * from the HTTP response body (JSON array) at any time.
     */
    @GetMapping("/all-sorted")
    public List<Concept> getAllSorted() {
        return service.getAll();
    }

    @GetMapping("/{id}")
    public Concept getById(@PathVariable Long id) {
        return service.getById(id);
    }

    @PutMapping("/{id}")
    public Concept update(@PathVariable Long id, @RequestBody ConceptDTO dto) throws IOException {
        return service.update(id, dto);
    }

    @DeleteMapping("/{id}")
    public void delete(@PathVariable Long id) throws IOException {
        service.delete(id);
    }

    @GetMapping("/search")
    public List<Concept> search(@RequestParam String keyword) {
        return service.search(keyword);
    }

    /**
     * Applies a traffic-light recall score to a concept.
     *
     * PATCH /concepts/{id}/review
     * Body: { "score": 2 }   // 2=easy (+2 str), 1=medium (+1 str), 0=tough (-2 str)
     *
     * Returns the updated Concept with new strength / repetitions / lastReviewed.
     */
    @PatchMapping("/{id}/review")
    public Concept review(@PathVariable Long id, @RequestBody ReviewDTO dto) {
        return service.applyReview(id, dto.getScore());
    }
}
