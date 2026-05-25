package com.verstappen.memorypalace.service;

import java.io.File;
import java.io.FileReader;
import java.io.InputStream;
import java.io.Reader;
import java.nio.file.Files;
import java.util.List;

import org.springframework.boot.CommandLineRunner;
import org.springframework.stereotype.Service;

import com.opencsv.bean.CsvToBean;
import com.opencsv.bean.CsvToBeanBuilder;
import com.verstappen.memorypalace.model.Concept;
import com.verstappen.memorypalace.repository.ConceptRepository;

@Service
public class CsvLoaderService implements CommandLineRunner {

    private final ConceptRepository repo;

    private static final String RESOURCE_PATH = "data/concepts.csv";
    private static final String RUNTIME_PATH = System.getProperty("user.dir") + "/data/concepts.csv";

    public CsvLoaderService(ConceptRepository repo) {
        this.repo = repo;
    }

    @Override
    public void run(String... args) {
        initializeCSV();
        loadDataIfEmpty();
    }

    // Step 1: Copy from resources → filesystem (ONLY FIRST TIME)
    private void initializeCSV() {
        try {
            File file = new File(RUNTIME_PATH);

            if (!file.exists()) {
                file.getParentFile().mkdirs();

                InputStream in = getClass().getClassLoader().getResourceAsStream(RESOURCE_PATH);
                Files.copy(in, file.toPath());

                System.out.println("CSV copied to runtime folder.");
            }

        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    // Step 2: Load into DB only if empty
    public void loadDataIfEmpty() {
        try {
            if (repo.count() > 0) {
                System.out.println("DB already loaded.");
                return;
            }

            Reader reader = new FileReader(RUNTIME_PATH);

            CsvToBean<Concept> csvToBean = new CsvToBeanBuilder<Concept>(reader)
                    .withType(Concept.class)
                    .withIgnoreLeadingWhiteSpace(true)
                    .build();

            List<Concept> concepts = csvToBean.parse();

            repo.saveAll(concepts);

            System.out.println("CSV → DB load complete.");

        } catch (Exception e) {
            e.printStackTrace();
        }
    }
}