package com.verstappen.memorypalace.service;

import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.time.LocalDateTime;
import java.util.List;

import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.stereotype.Service;

import com.verstappen.memorypalace.dto.ConceptDTO;
import com.verstappen.memorypalace.model.Concept;
import com.verstappen.memorypalace.repository.ConceptRepository;

@Service
public class ConceptService {

    private final ConceptRepository repo;

    private static final String CSV_FILE_PATH = System.getProperty("user.dir") + "/data/concepts.csv";

    public ConceptService(ConceptRepository repo) {
        this.repo = repo;
    }

    // =========================
    // CREATE
    // =========================
    public synchronized Concept save(ConceptDTO dto) throws IOException {
        Concept c = mapDTO(dto);
        Concept saved = repo.save(c);
        rewriteCSV();
        return saved;
    }

    // =========================
    // READ
    // =========================

    /**
     * Returns ALL concepts ordered by id ascending.
     * Called by GET /concepts/all-sorted — used by the frontend to extract
     * the full list in ascending order.
     */
    public List<Concept> getAll() {
        return repo.findAllByOrderByIdAsc();
    }

    public Concept getById(Long id) {
        return repo.findById(id)
                .orElseThrow(() -> new RuntimeException("Concept not found"));
    }

    // =========================
    // UPDATE
    // =========================
    public synchronized Concept update(Long id, ConceptDTO dto) throws IOException {
        Concept c = getById(id);

        c.setTitle(dto.getTitle());
        c.setDescription(dto.getDescription());
        c.setMediaUrl(dto.getMediaUrl());
        c.setMemoryObject(dto.getMemoryObject());
        c.setLocation(dto.getLocation());
        c.setVisualCue(dto.getVisualCue());

        Concept updated = repo.save(c);
        rewriteCSV();
        return updated;
    }

    // =========================
    // DELETE
    // =========================
    public synchronized void delete(Long id) throws IOException {
        repo.deleteById(id);
        rewriteCSV();
    }

    // =========================
    // REVIEW (traffic-light recall)
    // =========================
    /**
     * Applies a recall score to a concept and persists the updated
     * strength / repetitions / lastReviewed to the database.
     *
     * score 2 → +2 strength (easy)
     * score 1 → +1 strength (medium)
     * score 0 → −2 strength (tough)
     */
    public synchronized Concept applyReview(Long id, int score) {
        Concept c = getById(id);

        if (score == 2)      c.setStrength(c.getStrength() + 2);
        else if (score == 1) c.setStrength(c.getStrength() + 1);
        else                 c.setStrength(c.getStrength() - 2);

        c.setRepetitions(c.getRepetitions() + 1);
        c.setLastReviewed(LocalDateTime.now());

        return repo.save(c);
    }

    // =========================
    // SEARCH & PAGINATION
    // =========================
    public List<Concept> search(String keyword) {
        return repo.findByTitleContainingIgnoreCase(keyword);
    }

    public Page<Concept> getPaginated(Pageable pageable) {
        return repo.findAll(pageable);
    }

    // =========================
    // CSV SYNC LOGIC
    // =========================
    private void rewriteCSV() throws IOException {
        List<Concept> all = repo.findAllByOrderByIdAsc();
        File file = new File(CSV_FILE_PATH);
        file.getParentFile().mkdirs();

        try (FileWriter writer = new FileWriter(file)) {
            writer.append("id,title,description,mediaUrl,memoryObject,location,visualCue\n");
            for (Concept c : all) {
                writer.append(c.getId().toString()).append(",")
                        .append(safe(c.getTitle())).append(",")
                        .append(safe(c.getDescription())).append(",")
                        .append(safe(c.getMediaUrl())).append(",")
                        .append(safe(c.getMemoryObject())).append(",")
                        .append(safe(c.getLocation())).append(",")
                        .append(safe(c.getVisualCue())).append("\n");
            }
        }

        System.out.println("CSV synced with database.");
    }

    // =========================
    // HELPER METHODS
    // =========================
    private Concept mapDTO(ConceptDTO dto) {
        Concept c = new Concept();
        c.setTitle(dto.getTitle());
        c.setDescription(dto.getDescription());
        c.setMediaUrl(dto.getMediaUrl());
        c.setMemoryObject(dto.getMemoryObject());
        c.setLocation(dto.getLocation());
        c.setVisualCue(dto.getVisualCue());
        return c;
    }

    private String safe(String s) {
        if (s == null) return "";
        return "\"" + s.replace("\"", "\"\"") + "\"";
    }
}
